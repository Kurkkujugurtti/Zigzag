package fi.quanfoxes.parser.patterns;

import fi.quanfoxes.Keywords;
import fi.quanfoxes.lexer.KeywordToken;
import fi.quanfoxes.lexer.Token;
import fi.quanfoxes.lexer.TokenType;
import fi.quanfoxes.parser.Context;
import fi.quanfoxes.parser.Node;
import fi.quanfoxes.parser.Pattern;
import fi.quanfoxes.parser.Singleton;
import fi.quanfoxes.parser.nodes.ReturnNode;

import java.util.List;

public class ReturnPattern extends Pattern {
    public static final int PRIORITY = 1;

    private static final int RETURN = 0;
    private static final int OBJECT = 1;

    public ReturnPattern() {
        // Pattern:
        // return ...
        super(TokenType.KEYWORD, TokenType.FUNCTION | TokenType.IDENTIFIER | 
                    TokenType.NUMBER | TokenType.CONTENT | TokenType.DYNAMIC);
    }

    @Override
    public int priority(List<Token> tokens) {
        return PRIORITY;
    }

    @Override
    public boolean passes(List<Token> tokens) {
        KeywordToken keyword = (KeywordToken)tokens.get(RETURN);
        return keyword.getKeyword() == Keywords.RETURN;
    }

    @Override
    public Node build(Context context, List<Token> tokens) throws Exception {
        return new ReturnNode(Singleton.parse(context, tokens.get(OBJECT)));
    }
}
