import spacy
from spacy.matcher import Matcher
import re

# Load SpaCy English model
nlp = spacy.load("en_core_web_sm")

# Initialize matcher
matcher = Matcher(nlp.vocab)

# Define trait keywords
trait_mapping = {
    "temperature": ["temperature", "temp"],
    "rainfall": ["rainfall", "precipitation"],
    "humidity": ["humidity", "humid"]
}

# Add matcher patterns for only trait keywords
for trait, keywords in trait_mapping.items():
    for keyword in keywords:
        pattern = [{"LOWER": keyword.lower()}]
        matcher.add(trait, [pattern])

# Function to extract number or range
def extract_num_range(text):
    match = re.search(r"(\d+\.?\d*)\s*(?:[-–to]+\s*(\d+\.?\d*))?\s*([a-zA-Z°%]*)", text)
    if match:
        num1 = float(match.group(1))
        num2 = float(match.group(2)) if match.group(2) else None
        unit = match.group(3)
        return (num1, num2, unit)
    return None

# Function to find number/range after trait
def extract_traits_with_ranges(query):
    doc = nlp(query)
    matches = matcher(doc)
    extracted_traits = {}

    for match_id, start, end in matches:
        trait_name = nlp.vocab.strings[match_id]
        span = doc[start:end]
        
        # Search for numbers around the matched keyword
        next_tokens = doc[end:end+7]  # Look ahead 7 tokens (safe window)
        text_to_search = span.text + " " + " ".join([token.text for token in next_tokens])
        
        range_info = extract_num_range(text_to_search)
        if range_info:
            num1, num2, unit = range_info
            extracted_traits[trait_name] = {
                "min": num1,
                "max": num2 if num2 else num1,
                "unit": unit or None
            }

    return extracted_traits
