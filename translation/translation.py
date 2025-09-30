import os
import csv
import asyncio
import time
import re
from openai import AsyncOpenAI

# --- Configuration ---

# IMPORTANT: Set your OpenAI API key as an environment variable for security.
# How to set an environment variable:
# - On macOS/Linux: open Terminal and type `export OPENAI_API_KEY='your_api_key_here'`
# - On Windows: open Command Prompt and type `set OPENAI_API_KEY=your_api_key_here`
#   or use the System Properties > Environment Variables GUI.
# The script will then read this key automatically.
# If you prefer not to set an environment variable, you can uncomment the next line
# and paste your key directly, but this is less secure.
# os.environ["OPENAI_API_KEY"] = "your_api_key_here"

# Initialize the OpenAI client
# It will automatically look for the OPENAI_API_KEY environment variable.
try:
    client = AsyncOpenAI()
except Exception as e:
    print(f"Error initializing OpenAI client: {e}")
    print("Please ensure your OPENAI_API_KEY is set correctly as an environment variable.")
    exit()


# Define the input and output filenames
INPUT_CSV_FILE = 'input.csv'
OUTPUT_CSV_FILE = 'output.csv'

# --- Helper Functions ---

def contains_korean(text):
    """
    Check if the text contains any Korean characters.
    
    Args:
        text (str): The text to check
        
    Returns:
        bool: True if Korean characters are found, False otherwise
    """
    if not text or not text.strip():
        return False
    
    # Korean character ranges in Unicode
    korean_ranges = [
        (0xAC00, 0xD7AF),  # Hangul Syllables
        (0x1100, 0x11FF),  # Hangul Jamo
        (0x3130, 0x318F),  # Hangul Compatibility Jamo
        (0xA960, 0xA97F),  # Hangul Jamo Extended-A
        (0xD7B0, 0xD7FF),  # Hangul Jamo Extended-B
    ]
    
    for char in text:
        char_code = ord(char)
        for start, end in korean_ranges:
            if start <= char_code <= end:
                return True
    return False

def replace_commas_with_backticks(text):
    """
    Replace all commas with backticks in the given text.
    
    Args:
        text (str): The text to process
        
    Returns:
        str: Text with commas replaced by backticks
    """
    if not text:
        return text
    return text.replace(',', '`')

# --- Translation Prompt ---

# This is the detailed prompt you crafted.
# The script will append the Korean text to be translated to this prompt.
TRANSLATION_PROMPT_TEMPLATE = """
You are an expert translator specializing in localizing Korean video games into natural, fluent English. Your primary focus is on preserving the original intent, tone, and specific formatting required by the game's engine.

**Game Context:** The game is a psychological escape-room with a suspenseful, mysterious, and often scary narrative. The translations must reflect this atmosphere.

**Task:** Translate the following Korean script line into English. You must follow these critical rules precisely.

---

### CRITICAL RULES

1.  **Preserve Special Codes:** Do not translate, alter, or remove special coded placeholders. These must be kept exactly as they appear in the original text.
    * `{PlayerName}` must remain `{PlayerName}`.
    * The newline character `\\n` must remain `\\n`.
    * The backtick character `` ` `` must remain `` ` `` (this is important for CSV parsing).
    * **Note:** Korean grammatical particles attached to placeholders, like `(이)`, `(가)`, or `(을/를)`, should be dropped in the English translation. For example, `{PlayerName}(이)랑` becomes `with {PlayerName}`.

2.  **Preserve Brackets:** Any text inside square brackets `[ ]` is a special keyword for the game. You must translate the text *inside* the brackets but keep the brackets themselves.
    * **Example:** `[캐릭터 부적]` should be translated as `[character charm]`.

3.  **Handle "Scary Text" (No Spacing):** Some Korean lines are written without any spaces to create a sense of fear, panic, or intensity. For these specific lines, you must follow a two-step process:
    * **Step 1:** First, translate the Korean text into a natural, grammatically correct English sentence with proper spacing.
    * **Step 2:** Then, convert that entire translated English sentence to **ALL CAPS**. This maintains the intended scary or emphatic tone while ensuring readability in English.
    * **Example:**
        * **Original Korean:** `내가죽었으면좋겠어서그렇게한거야?`
        * **Correct English Output:** `DID YOU DO THAT BECAUSE YOU WANTED ME TO DIE?`

4. **Translate Specific Names:** There are two specific character names that must be translated consistently throughout the entire script. Do not translate them based on their literal meaning.
    * The Korean name 필연 must always be translated as Fate.
    * The Korean name 우연 must always be translated as Accidy.
    * **Example:**
        * **Original Korean:** `[필연] 엔딩`
        * **Correct English Output:** `[Fate] Ending`
    * **Example:**
        * **Original Korean:** `우연이 귀가했다.`
        * **Correct English Output:** `Accidy has returned home.`

---
Translate the following Korean text:

"""

# --- Core Functions ---

async def translate_text(korean_text, max_retries=3):
    """
    Sends the Korean text to the OpenAI API for translation with retry logic.

    Args:
        korean_text (str): The Korean script line to translate.
        max_retries (int): Maximum number of retry attempts for rate limits.

    Returns:
        str: The translated English text, or an error message if translation fails.
    """
    if not korean_text.strip():
        # Return an empty string if the source cell is empty
        return ""

    for attempt in range(max_retries + 1):
        try:
            # Construct the full prompt for the API call
            full_prompt = TRANSLATION_PROMPT_TEMPLATE + korean_text

            # Make the API call
            response = await client.chat.completions.create(
                model="gpt-5",
                messages=[
                    {"role": "user", "content": full_prompt}
                ],
                max_completion_tokens=1000
            )
            
            # Extract the translated text from the response
            translated_text = response.choices[0].message.content.strip()
            
            # Replace commas with backticks before returning
            translated_text = replace_commas_with_backticks(translated_text)
            
            return translated_text

        except Exception as e:
            error_str = str(e)
            
            # Check if it's a rate limit error
            if "rate_limit" in error_str.lower() or "429" in error_str:
                if attempt < max_retries:
                    # Extract wait time from error message if available
                    wait_time = 2 ** attempt  # Exponential backoff: 1s, 2s, 4s
                    
                    # Try to extract the suggested wait time from the error message
                    if "try again in" in error_str:
                        try:
                            import re
                            match = re.search(r'try again in (\d+\.?\d*)s', error_str)
                            if match:
                                wait_time = float(match.group(1)) + 0.5  # Add buffer
                        except:
                            pass
                    
                    print(f"Rate limit hit, waiting {wait_time:.1f}s before retry {attempt + 1}/{max_retries + 1} for: '{korean_text[:50]}...'")
                    await asyncio.sleep(wait_time)
                    continue
                else:
                    print(f"Max retries reached for '{korean_text[:50]}...': {e}")
                    return "---TRANSLATION ERROR (RATE LIMIT)---"
            else:
                # Non-rate-limit error
                print(f"An error occurred while translating '{korean_text[:50]}...': {e}")
                return "---TRANSLATION ERROR---"
    
    return "---TRANSLATION ERROR---"


async def process_csv_file(input_file, output_file, max_concurrent=3):
    """
    Reads the input CSV, translates the text concurrently, and writes to the output CSV with incremental saving.

    Args:
        input_file (str): The path to the source CSV file.
        output_file (str): The path where the translated CSV file will be saved.
        max_concurrent (int): Maximum number of concurrent API calls (default: 3).
    """
    print(f"Starting translation process for '{input_file}' with {max_concurrent} concurrent requests...")

    try:
        # Read all rows first
        with open(input_file, mode='r', encoding='utf-8') as infile:
            reader = csv.reader(infile)
            header = next(reader)
            rows = list(reader)
        
        print(f"Found {len(rows)} rows to process.")
        
        # Initialize output file with header
        with open(output_file, mode='w', encoding='utf-8', newline='') as outfile:
            writer = csv.writer(outfile)
            writer.writerow(header)
        print("Header written to output file.")

        # Process rows in batches to control concurrency
        semaphore = asyncio.Semaphore(max_concurrent)
        
        async def translate_with_semaphore(korean_script, row_index):
            """Only for actual API translation calls"""
            async with semaphore:
                print(f"Translating row {row_index + 1}: '{korean_script[:50]}...'")
                english_translation = await translate_text(korean_script)
                return row_index, korean_script, english_translation

        def process_row_sync(row_data, row_index):
            """Process rows that don't need API calls (synchronous)"""
            korean_script = row_data[0] if len(row_data) > 0 else ""
            existing_english = row_data[1] if len(row_data) > 1 else ""
            
            print(f"Processing row {row_index + 1}: '{korean_script[:50]}...'")
            
            # Rule 1: Skip if first column is empty
            if not korean_script or not korean_script.strip():
                print(f"  Skipping row {row_index + 1}: Empty first column")
                return row_index, korean_script, existing_english
            
            # Rule 3: Skip if second column already has content
            if existing_english and existing_english.strip():
                print(f"  Skipping row {row_index + 1}: Second column already has content")
                return row_index, korean_script, existing_english
            
            # Rule 2: If no Korean characters, copy directly
            if not contains_korean(korean_script):
                print(f"  Row {row_index + 1}: No Korean characters found, copying directly")
                return row_index, korean_script, korean_script
            
            # Return None to indicate this row needs API translation
            return None

        # Process rows in smaller batches for incremental saving
        batch_size = 10
        completed_count = 0
        start_time = time.time()
        batch_times = []
        
        for batch_start in range(0, len(rows), batch_size):
            batch_start_time = time.time()
            batch_end = min(batch_start + batch_size, len(rows))
            batch_rows = rows[batch_start:batch_end]
            
            print(f"\nProcessing batch {batch_start//batch_size + 1}: rows {batch_start + 1} to {batch_end}")
            
            # First, process all rows synchronously to identify which need API calls
            batch_translations = []
            api_tasks = []
            
            for i, row in enumerate(batch_rows):
                global_index = batch_start + i
                if not row:
                    # Handle completely empty rows
                    batch_translations.append((global_index, "", ""))
                    continue
                
                # Process row synchronously first
                result = process_row_sync(row, global_index)
                
                if result is None:
                    # This row needs API translation
                    korean_script = row[0] if len(row) > 0 else ""
                    task = translate_with_semaphore(korean_script, global_index)
                    api_tasks.append((global_index, task))
                else:
                    # This row was processed synchronously
                    batch_translations.append(result)

            # Execute API calls concurrently (only if there are any)
            if api_tasks:
                print(f"  Making {len(api_tasks)} API calls for this batch...")
                api_results = await asyncio.gather(*[task for _, task in api_tasks], return_exceptions=True)
                
                # Add API results to batch translations
                for i, (global_index, _) in enumerate(api_tasks):
                    result = api_results[i]
                    if isinstance(result, Exception):
                        print(f"Error in row {global_index + 1}: {result}")
                        batch_translations.append((global_index, batch_rows[global_index - batch_start][0] if batch_rows[global_index - batch_start] else "", "---TRANSLATION ERROR---"))
                    else:
                        batch_translations.append(result)
            else:
                print(f"  No API calls needed for this batch")

            # Sort batch results by row index
            batch_translations.sort(key=lambda x: x[0])

            # Append batch results to output file
            with open(output_file, mode='a', encoding='utf-8', newline='') as outfile:
                writer = csv.writer(outfile)
                for row_index, korean_script, english_translation in batch_translations:
                    writer.writerow([korean_script, english_translation])
                    completed_count += 1

            # Calculate timing and estimates
            batch_end_time = time.time()
            batch_duration = batch_end_time - batch_start_time
            batch_times.append(batch_duration)
            
            # Calculate average time per batch (excluding outliers)
            if len(batch_times) > 1:
                # Remove fastest and slowest batch for more accurate average
                sorted_times = sorted(batch_times)
                if len(sorted_times) > 2:
                    avg_batch_time = sum(sorted_times[1:-1]) / (len(sorted_times) - 2)
                else:
                    avg_batch_time = sum(batch_times) / len(batch_times)
            else:
                avg_batch_time = batch_duration
            
            # Calculate remaining batches and estimated time
            remaining_batches = (len(rows) - completed_count + batch_size - 1) // batch_size
            estimated_remaining_time = remaining_batches * avg_batch_time
            
            # Format time estimates
            def format_time(seconds):
                if seconds < 60:
                    return f"{seconds:.0f}s"
                elif seconds < 3600:
                    minutes = seconds / 60
                    return f"{minutes:.1f}m"
                else:
                    hours = seconds / 3600
                    return f"{hours:.1f}h"
            
            elapsed_time = time.time() - start_time
            print(f"Batch {batch_start//batch_size + 1} complete in {format_time(batch_duration)}. "
                  f"{completed_count}/{len(rows)} rows processed and saved.")
            print(f"Elapsed: {format_time(elapsed_time)} | "
                  f"Avg batch time: {format_time(avg_batch_time)} | "
                  f"ETA: {format_time(estimated_remaining_time)}")

        print(f"\nProcessing complete. Output saved to '{output_file}'.")

    except FileNotFoundError:
        print(f"Error: The file '{input_file}' was not found.")
        print("Please make sure your CSV file is named 'input.csv' and is in the same directory as the script.")
    except Exception as e:
        print(f"An unexpected error occurred: {e}")

# --- Main Execution ---

async def main():
    # Check if the API key is available
    if not os.getenv("OPENAI_API_KEY"):
        print("Error: The OPENAI_API_KEY environment variable is not set.")
        print("Please set the variable and run the script again.")
    else:
        await process_csv_file(INPUT_CSV_FILE, OUTPUT_CSV_FILE, max_concurrent=3)

if __name__ == "__main__":
    asyncio.run(main())
