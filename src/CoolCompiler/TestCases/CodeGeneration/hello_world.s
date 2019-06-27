.data
buff: .space 65536
substrexep: .asciiz "Substring index exception"
string0: .asciiz "Object"
string1: .asciiz "IO"
string2: .asciiz "Int"
string3: .asciiz "Bool"
string4: .asciiz "String"
string5: .asciiz "Main"
string6: .asciiz "Hello, World.\n"
_class.IO: .word string1, string0, 0
_class.Int: .word string2, string0, 0
_class.Bool: .word string3, string0, 0
_class.String: .word string4, string0, 0
_class.Main: .word string5, string1, string0, 0

.globl main
.text
_inherit:
lw $a0, 8($a0)
_inherit.cycle:
lw $a2, 0($a0)
beq $a1, $a2, _inherit.ok
beq $a2, $zero, _inherit.wrong
addiu $a0, $a0, 4
j _inherit.cycle
_inherit.wrong:
li $v0, 0
jr $ra
_inherit.ok:
li $v0, 0
jr $ra

_copy:
lw $a1, 0($sp)
lw $a0, -4($sp)
li $v0, 9
syscall
lw $a1, 0($sp)
lw $a0, 4($sp)
move $a3, $v0
_copy.cycle:
lw $a2, 0($a1)
sw $a2, 0($a3)
addiu $a0, $a0, -1
addiu $a1, $a1, 4
addiu $a3, $a3, 4
beq $a0, $zero, _copy.finish
j _copy.cycle
_copy.finish:
jr $ra

_abort:
li $v0, 10
syscall

_out_string:
li $v0, 4
lw $a0, 0($sp)
syscall
jr $ra

_out_int:
li $v0, 1
lw $a0, 0($sp)
syscall
jr $ra

_in_string:
move $a3, $ra
la $a0, buff
li $a1, 65536
li $v0, 8
syscall
addiu $sp, $sp, -4
sw $a0, 0($sp)
jal _strlen
addiu $sp, $sp, 4
move $a2, $v0
addiu $a2, $a2, -1
move $a0, $v0
li $v0, 9
syscall
move $v1, $v0
la $a0, buff
_in_string.cycle:
beqz $a2, _in_string.finish
lb $a1, 0($a0)
sb $a1, 0($v1)
addiu $a0, $a0, 1
addiu $v1, $v1, 1
addiu $a2, $a2, -1
j _in_string.cycle
_in_string.finish:
sb $zero, 0($v1)
move $ra, $a3
jr $ra

_in_int:
li $v0, 5
syscall
jr $ra

_strlen:
lw $a0, 0($sp)
_strlen.cycle:
lb $a1, 0($sp)
beqz $a0, _strlen.finish
addiu $a0, $a0, 1
j _strlen.cycle
_strlen.finish:
lw $a1, 0($sp)
subu $v0, $a0, $a1
jr $ra

_strcat:
move $a2, $ra
jal _strlen
move $v1, $v0
addiu $sp, $sp, -4
jal _strlen
addiu $sp, $sp, 4
add $v1, $v1, $v0
addi $v1, $v1, 1
li $v0, 9
move $a0, $v1
syscall
move $v1, $v0
lw $a0, 0($sp)
_strcat.firstcycle:
lb $a1, 0($a0)
beqz $a1, _strcat.firstexit
sb $a1, 0($v1)
addiu $a0, $a0, 1
addiu $v1, $v1, 1
j _strcat.firstcycle
_strcat.firstexit:
lw $a0, -4($sp)
_strcat.sndcycle:
lb $a1, 0($a0)
beqz $a1, _strcat.sndexit
sb $a1, 0($v1)
addiu $a0, $a0, 1
addiu $v1, $v1, 1
j _strcat.sndcycle
_strcat.sndexit:
sb $zero, 0($v1)
move $ra, $a2
jr $ra

_substr:
lw $a0, -8($sp)
addiu $a0, $a0, 1
li $v0, 9
syscall
move $v1, $v0
lw $a0, 0($sp)
lw $a1, -4($sp)
add $a0, $a0, $a1
lw $a2, -8($sp)
_substr.cycle:
beqz $a2, _substr.finish
lb $a1, 0($v1)
beqz $a1, _substrE
sb $a1, 0($v1)
addiu $a0, $a0, 1
addiu $v1, $v1, 1
addiu $a2, $a2, -1
j _substr.cycle
_substr.finish:
sb $zero, 0($v1)
jr $ra

_substrE:
la $a0, substrexep
li $v0, 4
syscall
li $v0, 10
syscall
_strcmp:
li $v0, 1
_strcmp.cycle:
lb $a2, 0($a0)
lb $a3, 0($a1)
beqz $a2, _strcmp.finish
beq $a2, $zero, _strcmp.finish
beq $a3, $zero, _strcmp.finish
bne $a2, $a3, _strcmp.neq
addiu $a0, $a0, 1
addiu $a1, $a1, 1
j _strcmp.cycle
_strcmp.finish:
beq $a2, $a3, _strcmp.eq
_strcmp.neq:
li $v0, 0
jr $ra
_strcmp.eq:
li $v0, 1
jr $ra

main:
sw $ra, 0($sp)
addiu $sp, $sp, -4
jal start
addiu $sp, $sp, 4
lw $ra, 0($sp)


Object.ctor:
li $t9, 0
la $a0, Object.abort
lw $a1, 0($sp)
sw $a0, 12($a1)
la $a0, Object.type_name
lw $a1, 0($sp)
sw $a0, 16($a1)
la $a0, Object.copy
lw $a1, 0($sp)
sw $a0, 20($a1)
la $a0, string0
lw $a1, 0($sp)
sw $a0, 0($a1)
lw $a0, 0($sp)
li $a1, 6
sw $a1, 4($a0)
lw $v0, 4($sp)
jr $ra


IO.ctor:
li $t9, 0
lw $a0, 0($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal Object.ctor
addiu $sp, $sp, 12
lw $ra, -8($sp)
la $a0, IO.out_string
lw $a1, 0($sp)
sw $a0, 24($a1)
la $a0, IO.out_int
lw $a1, 0($sp)
sw $a0, 28($a1)
la $a0, IO.in_string
lw $a1, 0($sp)
sw $a0, 32($a1)
la $a0, IO.in_int
lw $a1, 0($sp)
sw $a0, 36($a1)
la $a0, string1
lw $a1, 0($sp)
sw $a0, 0($a1)
lw $a0, 0($sp)
li $a1, 10
sw $a1, 4($a0)
la $a0, _class.IO
lw $a1, 0($sp)
sw $a0, 8($a1)
lw $v0, 4($sp)
jr $ra


_wrapper.Int:
li $t9, 0
li $v0, 9
li $a0, 28
syscall
sw $v0, -4($sp)
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal Object.ctor
addiu $sp, $sp, 12
lw $ra, -8($sp)
sw $v0, 0($sp)
la $a0, string2
lw $a1, -4($sp)
sw $a0, 0($a1)
lw $a0, 0($sp)
lw $a1, -4($sp)
sw $a0, 24($a1)
la $a0, _class.Int
lw $a1, -4($sp)
sw $a0, 8($a1)
lw $v0, -4($sp)
jr $ra


_wrapper.Bool:
li $t9, 0
li $v0, 9
li $a0, 28
syscall
sw $v0, -4($sp)
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal Object.ctor
addiu $sp, $sp, 12
lw $ra, -8($sp)
sw $v0, 0($sp)
la $a0, string3
lw $a1, -4($sp)
sw $a0, 0($a1)
lw $a0, 0($sp)
lw $a1, -4($sp)
sw $a0, 24($a1)
la $a0, _class.Bool
lw $a1, -4($sp)
sw $a0, 8($a1)
lw $v0, -4($sp)
jr $ra


_wrapper.String:
li $t9, 0
li $v0, 9
li $a0, 40
syscall
sw $v0, -4($sp)
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal Object.ctor
addiu $sp, $sp, 12
lw $ra, -8($sp)
sw $v0, 0($sp)
la $a0, string4
lw $a1, -4($sp)
sw $a0, 0($a1)
lw $a0, 0($sp)
lw $a1, -4($sp)
sw $a0, 36($a1)
la $a0, _class.String
lw $a1, -4($sp)
sw $a0, 8($a1)
lw $v0, -4($sp)
jr $ra


Object.abort:
li $t9, 0
j _abort


Object.type_name:
li $t9, 0
lw $a0, 0($sp)
lw $a1, 0($a0)
sw $a1, 0($sp)
lw $v0, 0($sp)
jr $ra


Object.copy:
li $t9, 0
lw $a0, 0($sp)
lw $a1, 4($a0)
sw $a1, -4($sp)
li $a0, 4
sw $a0, -8($sp)
lw $a0, -4($sp)
lw $a1, -8($sp)
mult $a0, $a1
mflo $a0
sw $a0, -4($sp)
lw $a0, 0($sp)
sw $a0, -16($sp)
lw $a0, -4($sp)
sw $a0, -20($sp)
sw $ra, -12($sp)
addiu $sp, $sp, -16
jal _copy
addiu $sp, $sp, 16
lw $ra, -12($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


IO.out_string:
li $t9, 0
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal _out_string
addiu $sp, $sp, 12
lw $ra, -8($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


IO.out_int:
li $t9, 0
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal _out_int
addiu $sp, $sp, 12
lw $ra, -8($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


IO.in_string:
li $t9, 0
sw $ra, -4($sp)
addiu $sp, $sp, -8
jal _in_string
addiu $sp, $sp, 8
lw $ra, -4($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


IO.in_int:
li $t9, 0
sw $ra, -4($sp)
addiu $sp, $sp, -8
jal _in_int
addiu $sp, $sp, 8
lw $ra, -4($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


String.length:
li $t9, 0
lw $a0, 0($sp)
sw $a0, -8($sp)
sw $ra, -4($sp)
addiu $sp, $sp, -8
jal _strlen
addiu $sp, $sp, 8
lw $ra, -4($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


String.concat:
li $t9, 0
lw $a0, 0($sp)
sw $a0, -12($sp)
lw $a0, -4($sp)
sw $a0, -16($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal _strcat
addiu $sp, $sp, 12
lw $ra, -8($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


String.substr:
li $t9, 0
lw $a0, 0($sp)
sw $a0, -16($sp)
lw $a0, -4($sp)
sw $a0, -20($sp)
lw $a0, -8($sp)
sw $a0, -24($sp)
sw $ra, -12($sp)
addiu $sp, $sp, -16
jal _substr
addiu $sp, $sp, 16
lw $ra, -12($sp)
sw $v0, 0($sp)
lw $v0, 0($sp)
jr $ra


Main.main:
li $t9, 0
lw $a0, 0($sp)
sw $a0, -4($sp)
la $a0, string6
sw $a0, -12($sp)
lw $a0, -4($sp)
lw $a1, 24($a0)
sw $a1, -8($sp)
lw $a0, -4($sp)
sw $a0, -20($sp)
lw $a0, -12($sp)
sw $a0, -24($sp)
sw $ra, -16($sp)
lw $a0, -8($sp)
addiu $sp, $sp, -20
jalr $ra, $a0
addiu $sp, $sp, 20
lw $ra, -16($sp)
sw $v0, -4($sp)
lw $v0, -4($sp)
jr $ra


Main.ctor:
li $t9, 0
lw $a0, 0($sp)
sw $a0, -8($sp)
sw $ra, -4($sp)
addiu $sp, $sp, -8
jal IO.ctor
addiu $sp, $sp, 8
lw $ra, -4($sp)
la $a0, Main.main
lw $a1, 0($sp)
sw $a0, 40($a1)
la $a0, string5
lw $a1, 0($sp)
sw $a0, 0($a1)
lw $a0, 0($sp)
li $a1, 11
sw $a1, 4($a0)
la $a0, _class.Main
lw $a1, 0($sp)
sw $a0, 8($a1)
lw $v0, 4($sp)
jr $ra


start:
li $t9, 0
li $v0, 9
li $a0, 44
syscall
sw $v0, -4($sp)
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal Main.ctor
addiu $sp, $sp, 12
lw $ra, -8($sp)
lw $a0, -4($sp)
sw $a0, -12($sp)
sw $ra, -8($sp)
addiu $sp, $sp, -12
jal Main.main
addiu $sp, $sp, 12
lw $ra, -8($sp)
li $v0, 10
syscall
