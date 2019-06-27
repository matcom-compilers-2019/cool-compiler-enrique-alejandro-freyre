#Reporte

##Integrantes
Enrique Cobreiro Suarez       C-411
Alejandro Dominguez Rivero    C-411
Alejandro Freyre Echevarria   C-412

##Arquitectura

El Compilador se consta de 5 modulos. El Lexer, que tokeniza un código escrito en Cool y nos brinda,
en caso de que existan, los errores. El Parser el cual nos transforma los tokens del Lexer en el AST.
El tercer módulo está dedicado al Análisis Semántico sobre el AST logrado en el Parser, durante este
proceso el compilador, está diseñado para la notificación de los errores. Por otra parte está la generación de Código
Intermedio (CIL) y la generacion de Código MIPS.

###Lexer y Parser

Las implementaciones tanto del lexer como del parser fueron realizadas utilizando Antlr. Como gramatica
base se utilizo la especifcada en el manual de COOL, a la cual se le realizaron pequeños cambios para
algunos detalles en específico.

###Análisis Semántico

El Análisis Semántico se realizó con dos recorridos al AST. El primero para registrar las Clases, sus
Atributos y sus Métodos, así como verificar la no existencia de herencia cíclica y la herencia a los
tipos Int, Bool, String. El Segundo para la analizar el cuerpo de los métodos y su tipo de retorno.

Durante este proceso nos enfrentamos problemas debido a la idea con la que trabajamos, en la cual cada
nodo del AST sabia como hacer su propio análisis semántico. También al tratar de recopilar la mayor
cantidad de errores posibles. En esta dividimos los errores en tipos donde un grupo de errores se catalogaban
como críticos, y era completamente innecesario continuar con el chequeo, y otros donde se segue con el
chequeo para una mayor detección de estos.

Para el manejo de la informacion recopilada durante este proceso se implentaron algunas clases a las que se
atribuyo este comportamiento(Scope, TypeInfo). Estas a partir de diccionarios y operaciones sobre estos 
logran satisfactoriamente su objetivo.

###Generación de Código Intermedio
Para evitar directamente el trabajo de generar código MIPS se diseñó una estructura de codigo intermedio que 
permitiese representar operaciones como el paso de parámetros y el trabajo con la memoria.
Dentro de la estructura del código intermedio se definen clases para la asignación de variables, memoria,
herencia, etc. Una vez definida la estructura de código intermedio seguimos el patrón que se uso para el chequeo
semántico, en el que cada nodo del AST sabe generar su propio código intermedio, o sea, que un ProgramNode genera
la parte de código intermedio que le corresponde y llama a las clases que lo componen para que estas a su vez
generen el código intermedio correspondente. La generación de código intermedio, se realiza entonces en profundidad
por cada rama del ProgramNode del AST.

###Generación de Código MIPS
Siguiendo la idea de que cada nodo sepa chequearse y generar su código intermedio, añadimos a cada nodo de la estructura
de código intermedio un metod que retornase todas las lineas de código MIPS correspondientes a la instrucción que se
intenta representar con dicho nodo. Para ello se llama a la clase WMIPS que tiene un conjunto de funciones que generan
la linea correspondiente de código MIPS para una instruccion determinada.
Antes de generar el código MIPS primero se realiza un chequeo por todos los nodos de código intermedio que se generan
a partir del ProgramNode, contabilizando parámetros y analizando la herencia, y los métodos dentro de cada clase. Una vez
concluída esta primera revisión se realiza una segunda revision para garantizar los offsets de los nodos que representan
asignación de memoria. Con los resultados de la primera revisión se inicializa la sección de datos y después se procede a 
generar las funciones _in_int, _in_string, _out_int, _out_string, _ abort, y algunas funciones para el manejo de strings.
Por último recorremos todos los nodos de código intermedio que generó ProgramNode llamando a la función propia de cada uno
para que esta genere el código MIPS que le corresponde.
