from code import extract_traits_with_ranges

# Test cases
test_queries = [
    "The temperature should be between 20-30°C",
    "We need rainfall of 15 to 25 mm",
    "Humidity levels should be 50-60%",
    "Temperature around 25.5°C is ideal"
]

# Test each query
for query in test_queries:
    print(f"\nQuery: {query}")
    results = extract_traits_with_ranges(query)
    print("Extracted traits:", results) 