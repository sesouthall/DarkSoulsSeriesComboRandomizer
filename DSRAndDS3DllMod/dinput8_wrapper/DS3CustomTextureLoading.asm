EXTERNDEF custom_texture_load:PROC
EXTERNDEF ?AtomicIncrementAddress@@3_KA:QWORD
EXTERNDEF ?FUN_140e60160Address@@3_KA:QWORD
EXTERNDEF ?FUN_140e5f650Address@@3_KA:QWORD
EXTERNDEF ?ContinueTextureLoadingAtAddress@@3_KA:QWORD
EXTERNDEF ?DAT_144799990Address@@3_KA:QWORD

.data

	MENU_Icon_00002 db "MENU_Icon_00002",0
	MENU_DummyIcon_00002 db "MENU_DummyIcon_00002",0
	?AtomicIncrementAddress@@3_KA QWORD 0
	?FUN_140e60160Address@@3_KA QWORD 0
	?FUN_140e5f650Address@@3_KA QWORD 0
	?ContinueTextureLoadingAtAddress@@3_KA QWORD 0
	?DAT_144799990Address@@3_KA QWORD 0

.code

custom_texture_load PROC
	call [?FUN_140e5f650Address@@3_KA]
	LEA RAX,[RSP+32]
	MOV [RSP+48],RAX
	MOV RCX,[RDI]
	MOV [RSP+32],RCX
	TEST RCX,RCX
	JZ RSPPlus32IsZero
	ADD RCX,8
	CALL [?AtomicIncrementAddress@@3_KA]
	NOP
RSPPlus32IsZero:
	LEA RDX,MENU_Icon_00002
	MOV RCX,RBX
	CALL [?FUN_140e60160Address@@3_KA]
	MOV R9,RAX
	LEA R8,MENU_DummyIcon_00002
	LEA RDX,[RSP+32]
	MOV RCX,RBX
	CALL [?FUN_140e5f650Address@@3_KA]
	JMP [?ContinueTextureLoadingAtAddress@@3_KA]
	MOV RAX,[?DAT_144799990Address@@3_KA]
	TEST AL,1
custom_texture_load ENDP

END