namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public static class DS2SotFSAssembly
    {
        public static byte[] GetItem = CommentedAssemblyParser.LoadDefuseOutput(GetItemAssembly);

        private const string GetItemAssembly = @"0:  48 83 ec 28             sub    rsp,0x28
4:  41 b8 08 00 00 00       mov    r8d,0x8
a:  49 bf 00 00 00 00 ff    movabs r15,0xffffffff00000000 ;Item Struct Address
11: ff ff ff
14: 49 8d 17                lea    rdx,[r15]
17: 48 b9 00 00 00 00 ff    movabs rcx,0xffffffff00000000 ;Item bag?
1e: ff ff ff
21: 45 31 c9                xor    r9d,r9d
24: 49 be 00 00 00 00 ff    movabs r14,0xffffffff00000000 ;Call add item function DarkSoulsII.exe+1A8C67
2b: ff ff ff
2e: 41 ff d6                call   r14
31: 48 83 c4 28             add    rsp,0x28
35: c3                      ret";

        public static byte[] RemoveItem = CommentedAssemblyParser.LoadDefuseOutput(RemoveItemAssembly);

        private const string RemoveItemAssembly = @"0:  48 83 ec 28             sub    rsp,0x28
4:  48 b9 fe fe fe fe fe    movabs rcx,0xfefefefefefefefe
b:  fe fe fe
e:  ba fe fe fe fe          mov    edx,0xfefefefe
13: 41 b8 01 00 00 00       mov    r8d,0x1
19: 48 b8 fe fe fe fe fe    movabs rax,0xfefefefefefefefe
20: fe fe fe
23: ff d0                   call   rax
25: 48 83 c4 28             add    rsp,0x28
29: c3                      ret";

        public static byte[] BonfireWarp = CommentedAssemblyParser.LoadDefuseOutput(BonfireWarpAssembly);

        private const string BonfireWarpAssembly = @"0:  48 81 ec 10 01 00 00    sub    rsp,0x110
7:  48 ba 00 00 00 00 ff    movabs rdx,0xffffffff00000000
e:  ff ff ff
11: 0f b7 12                movzx  edx,WORD PTR [rdx]
14: 48 8d 4c 24 50          lea    rcx,[rsp+0x50]
19: 41 b8 02 00 00 00       mov    r8d,0x2
1f: 49 be 00 00 00 00 ff    movabs r14,0xffffffff00000000
26: ff ff ff
29: 41 ff d6                call   r14
2c: 48 b9 00 00 00 00 ff    movabs rcx,0xffffffff00000000
33: ff ff ff
36: 48 89 c2                mov    rdx,rax
39: 49 be 00 00 00 00 ff    movabs r14,0xffffffff00000000
40: ff ff ff
43: 41 ff d6                call   r14
46: 48 81 c4 10 01 00 00    add    rsp,0x110
4d: c3                      ret ";

        public static byte[] ReadEventFlag = CommentedAssemblyParser.LoadDefuseOutput(ReadEventFlagAssembly);

        private const string ReadEventFlagAssembly = @"0:  48 83 ec 1c             sub    rsp,0x1c
4:  48 b9 fe fe fe fe fe    movabs rcx,0xfefefefefefefefe
b:  fe fe fe
e:  ba fe fe fe fe          mov    edx,0xfefefefe
13: 48 b8 fe fe fe fe fe    movabs rax,0xfefefefefefefefe
1a: fe fe fe
1d: ff d0                   call   rax
1f: 48 bb fe fe fe fe fe    movabs rbx,0xfefefefefefefefe
26: fe fe fe
29: 88 03                   mov    BYTE PTR [rbx],al
2b: 48 83 c4 1c             add    rsp,0x1c
2f: c3                      ret";

        public static byte[] WriteEventFlag = CommentedAssemblyParser.LoadDefuseOutput(WriteEventFlagAssembly);

        private const string WriteEventFlagAssembly = @"0:  48 83 ec 1c             sub    rsp,0x1c
4:  48 b9 fe fe fe fe fe    movabs rcx,0xfefefefefefefefe
b:  fe fe fe
e:  ba fe fe fe fe          mov    edx,0xfefefefe
13: 41 b8 fe 00 00 00       mov    r8d,0xfe
19: 48 b8 fe fe fe fe fe    movabs rax,0xfefefefefefefefe
20: fe fe fe
23: ff d0                   call   rax
25: 48 83 c4 1c             add    rsp,0x1c
29: c3                      ret";
    }
}