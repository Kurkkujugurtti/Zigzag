package fi.quanfoxes.lexer;

import fi.quanfoxes.Keywords;

import java.util.ArrayList;

public class Lexer {

    private static final String OPENING_PARENTHESIS = "{[(";
    private static final String CLOSING_PARENTHESIS  = "}])";

    public static enum Type {
        UNSPECIFIED,
        TEXT,
        NUMBER,
        CONTENT,
        OPERATOR
    }

    public static class Area {
        public Type type;

        public String text;

        public Position start;
        public Position end;
    }

    public static class Position {
        private int line;
        private int character;

        private int absolute;

        public Position() {
            this(0, 0, 0);
        }

        public Position(int line, int character, int absolute) {
            this.line = line;
            this.character = character;
            this.absolute = absolute;
        }

        public void nextLine() {
            line++;
            character = 0;
            absolute++;
        }

        public void nextCharacter() {
            character++;
            absolute++;
        }

        public int getLine() {
            return line;
        }

        public int getCharacter() {
            return character;
        }

        public int getAbsolute() {
            return absolute;
        }

        public Position clone() {
            return new Position(line, character, absolute);
        }
    }

    private static boolean isOperator (char c) {
        return c >= 33 && c <= 47 || c >= 58 && c <= 63 || c == 94 || c == 124 || c == 126;
    }

    private static boolean isDigit(char c) {
        return c >= 48 && c <= 57;
    }

    private static boolean isText(char c) {
        return c >= 65 && c <= 90 || c >= 97 && c <= 122 || c == 95;
    }

    private static boolean isContent (char c) {
        return OPENING_PARENTHESIS.indexOf(c) >= 0;
    }

    private static Type getType (char c) {

        if (isText(c)) {
            return Type.TEXT;
        }
        else if (isDigit(c)) {
            return Type.NUMBER;
        }
        else if (isContent(c)) {
            return Type.CONTENT;
        }
        else if (isOperator(c)) {
            return Type.OPERATOR;
        }

        return Type.UNSPECIFIED;
    }

    private static boolean isPartOf (Type base, Type current, char c) {
        if (current == base || base == Type.UNSPECIFIED) {
            return true;
        }

        switch (base) {
            case TEXT:
                return current == Type.NUMBER;
            case NUMBER:
                return c == '.';
            default:
                return false;
        }
    }

    private static Position skipSpaces(String text, Position position) {

        while (position.getAbsolute() < text.length()) {
            char c = text.charAt(position.absolute);

            if (!Character.isSpaceChar(c)) {
                break;
            }
            else if (c == '\n') {
                position.nextLine();
            }
            else {
                position.nextCharacter();
            }
        }

        return position;
    }

    private static int skipSpaces(String text, int position) {

        while (position < text.length() && Character.isSpaceChar(text.charAt(position))) {
            position++;
        }

        return position;
    }

    private static Position skipContent(String text, Position start) throws Exception {

        Position position = start.clone();

        char opening = text.charAt(position.getAbsolute());
        char closing = CLOSING_PARENTHESIS.charAt(OPENING_PARENTHESIS.indexOf(opening));
        
        int count = 0;

        while (position.getAbsolute() < text.length()) {
            char c = text.charAt(position.getAbsolute());

            if (c == '\n') {
                position.nextLine();
            }
            else {
                if (c == opening) {
                    count++;
                }
                else if (c == closing) {
                    count--;
                }

                position.nextCharacter();
            }

            if (count == 0) {
                return position;
            }
        }

        throw new Exception("Couldn't find closing parenthesis");
    }

    private static int skipContent(String text, int start) throws Exception {

        int count = 0;
        int position = start;

        char opening = text.charAt(position);
        char closing = CLOSING_PARENTHESIS.charAt(OPENING_PARENTHESIS.indexOf(opening));

        while (position < text.length()) {
            char c = text.charAt(position);

            if (c == opening) {
                count++;
            }
            else if (c == closing) {
                count--;
            }

            position++;

            if (count == 0) {
                return position;
            }
        }

        throw new Exception("Couldn't find closing parenthesis");
    }

    public static Area getNextToken(String text, Position start) throws Exception {

        //int position = skipSpaces(text, start);
        Position position = skipSpaces(text, start);

        if (position.getAbsolute() == text.length()) {
            return null;
        }

        Area area = new Area();
        area.start = position.clone();
        area.type = getType(text.charAt(position.getAbsolute()));

        // Content area can be determined
        if (area.type == Type.CONTENT) {
            area.end = skipContent(text, area.start);
            area.text = text.substring(area.start.getAbsolute(), area.end.getAbsolute());
            return area;
        }

        // Possible types are now: TEXT, NUMBER, OPERATOR
        while (position.getAbsolute() < text.length()) {

            char c = text.charAt(position.getAbsolute());

            if (isContent(c)) {

                // There cannot be number and content tokens side by side
                if (area.type == Type.NUMBER) {
                    throw new Exception("Missing operator between number and parenthesis");
                }

                break;
            }

            Type type = getType(c);

            if (!isPartOf(area.type, type, c)) {
                break;
            }

            if (c == '\n') {
                position.nextLine();
            }
            else {
                position.nextCharacter();
            }
        }

        area.end = position;
        area.text = text.substring(area.start.getAbsolute(), area.end.getAbsolute());

        return area;
    }

    private static Token parseTextToken (String text) {

        if (OperatorType.has(text)) {
            return new OperatorToken(text);
        }
        else if (Keywords.exists(text)) {
            return new KeywordToken(text);
        }
        else {
            return new IdentifierToken(text);
        }
    }

    private static Token parseToken (Area area) throws Exception {
        switch (area.type) {
            case TEXT:
                return parseTextToken(area.text);
            case NUMBER:
                return new NumberToken(area.text);
            case OPERATOR:
                return new OperatorToken(area.text);
            case CONTENT:
                return new ContentToken(area.text);
            default:
                throw new Exception("Unrecognized token");
        }
    }

    private static final int FUNCTION_LENGTH = 2;

    private static void functions(ArrayList<Token> tokens) {
        if (tokens.size() < FUNCTION_LENGTH) {
            return;
        }

        for (int i = tokens.size() - 2; i >= 0;) {
            Token current = tokens.get(i);

            if (current.getType() == TokenType.IDENTIFIER) {
                Token next = tokens.get(i + 1);

                if (next.getType() == TokenType.CONTENT) {
                    ContentToken parameters = (ContentToken)next;

                    if (parameters.getParenthesisType() == ParenthesisType.PARENTHESIS) {
                        IdentifierToken name = (IdentifierToken)current;
                        FunctionToken function = new FunctionToken(name, parameters);
                    
                        tokens.set(i, function);
                        tokens.remove(i + 1);

                        i -= FUNCTION_LENGTH;
                        continue;
                    }
                }
            }

            i--;
        }
    }

    public static ArrayList<Token> getTokens (String section) throws Exception {
        //section = section.trim();

        ArrayList<Token> tokens = new ArrayList<>();
        Position position = new Position();

        while (position.getAbsolute() < section.length()) {
            Area area = getNextToken(section, position);

            if (area == null) {
                break;
            }

            Token token = parseToken(area);
            token.setPosition(area.start);
            tokens.add(token);

            position = area.end;
        }

        functions(tokens);

        return tokens;
    }
}