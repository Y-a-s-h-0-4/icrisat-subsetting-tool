from flask import Flask, request, jsonify
from flask_cors import CORS
from code import extract_traits_with_ranges

app = Flask(__name__)
CORS(app, resources={r"/*": {"origins": "http://localhost:5102"}})

@app.route('/extract-traits', methods=['POST'])
def extract_traits():
    """
    Extract traits and their values from the given text.
    Returns a JSON response with the extracted traits.
    """
    data = request.get_json()
    if not data or 'text' not in data:
        return jsonify({'error': 'No text provided'}), 400
    
    text = data['text']
    traits = extract_traits_with_ranges(text)
    return jsonify({'traits': traits})


if __name__ == '__main__':
    app.run(host='127.0.0.1', port=5000) 