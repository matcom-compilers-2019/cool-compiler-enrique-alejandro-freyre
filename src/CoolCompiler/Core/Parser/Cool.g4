grammar Cool;

/*
 * Parser Rules
 */
 
program : (classdef ';')+ EOF
		  ;

classdef: CLASS TYPE (INHERITS TYPE)? '{' (feature ';')* '}'
		;

feature : method
		  | property
		  ;

method : ID '(' (formal(',' formal)*)* ')' ':' TYPE '{' expr '}'
		 ;

property : formal (ASSIGNMENT expr)?
		   ;

formal : ID ':' TYPE;

expr      :       expr ('@' TYPE)? '.' ID '(' (expr (',' expr)*)* ')'           #dispatchExplicit
                |       ID '(' (expr (',' expr)*)* ')'                          #dispatchImplicit
                |       IF expr THEN expr ELSE expr FI                          #if
                |       WHILE expr LOOP expr POOL                               #while
                |       '{' (expr ';')+ '}'                                     #block
                |       CASE expr OF (formal IMPLY expr ';')+ ESAC				#case
                |       NEW TYPE                                                #new
                |       '~' expr                                                #negative
                |       ISVOID expr                                             #isvoid
                |       expr op=('*' | '/') expr                                #arithmetic
                |       expr op=('+' | '-') expr                                #arithmetic
                |       expr op=('<=' | '<' | '=') expr                         #comparisson
                |       NOT expr                                                #boolNot
                |       '(' expr ')'                                            #parentheses
                |       ID                                                      #id
                |       INT                                                     #int
                |       STRING                                                  #string
                |       value=(TRUE | FALSE)                                    #boolean
                |       ID ASSIGNMENT expr										#assignment
                |       LET property (',' property)* IN expr					#letIn
				;

// whitespaces, tabs, newlines skipping
WHITESPACE : [ \t\r\n\f]+ -> skip; 

// Comments
BLOCK_COMMENT   :   '(*' (BLOCK_COMMENT|.)*? '*)' -> channel(HIDDEN);
LINE_COMMENT    :   '--' .*? '\n' -> channel(HIDDEN);

// Keywords
CLASS: C L A S S;
ELSE: E L S E;
FALSE: 'f' A L S E;
FI: F I ;
IF: I F;
IN: I N;
INHERITS: I N H E R I T S;
ISVOID: I S V O I D;
LET: L E T;
LOOP: L O O P;
POOL: P O O L;
THEN: T H E N;
WHILE: W H I L E;
CASE: C A S E;
ESAC: E S A C;
NEW: N E W;
OF: O F;
NOT: N O T;
TRUE: 't' R U E;

// Ints, Types, Ids, Assignment, Imply
INT  :  ('0'..'9')+;
ID  :  ('a'..'z')('0'..'9'|'A'..'Z'|'a'..'z'|'_')*;
ASSIGNMENT  :  '<-';
IMPLY  :  '=>';
TYPE : ('A'..'Z')('0'..'9'|'A'..'Z'|'a'..'z'|'_')*;

// Strings
STRING : '"' (ESC | ~ ["\\])* '"';

fragment A: ('a' | 'A');
fragment C: ('c' | 'C');
fragment D: ('d' | 'D');
fragment E: ('e' | 'E');
fragment F: ('f' | 'F');
fragment H: ('h' | 'H');
fragment I: ('i' | 'I');
fragment L: ('l' | 'L');
fragment N: ('n' | 'N');
fragment O: ('o' | 'O');
fragment P: ('p' | 'P');
fragment R: ('r' | 'R');
fragment S: ('s' | 'S');
fragment T: ('t' | 'T');
fragment U: ('u' | 'U');
fragment V: ('v' | 'V');
fragment W: ('w' | 'W');

fragment ESC: '\\' (["\\/ntfb] | UNICODE);
fragment UNICODE: 'u' HEX HEX HEX HEX;
fragment HEX: ('0'..'9'|'a'..'f'|'A'..'F');
