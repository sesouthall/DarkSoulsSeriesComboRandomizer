namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    static class DSRAssembly
    {
        public static byte[] GetItem = CommentedAssemblyParser.LoadDefuseOutput(GetItemAssembly);

        private const string GetItemAssembly = @"0:  ba fe fe fe fe          mov    edx,0xfefefefe
5:  41 b9 fe fe fe fe       mov    r9d,0xfefefefe
b:  41 b8 fe fe fe fe       mov    r8d,0xfefefefe
11: 41 bc fe fe fe fe       mov    r12d,0xfefefefe
17: 48 a1 fe fe fe fe fe    movabs rax,ds:0xfefefefefefefefe
1e: fe fe fe
21: c6 44 24 38 01          mov    BYTE PTR [rsp+0x38],0x1
26: 40 88 7c 24 30          mov    BYTE PTR [rsp+0x30],dil
2b: c6 44 24 28 01          mov    BYTE PTR [rsp+0x28],0x1
30: 4c 8b 78 10             mov    r15,QWORD PTR [rax+0x10]
34: c6 44 24 20 01          mov    BYTE PTR [rsp+0x20],0x1
39: 49 8d 8f 80 02 00 00    lea    rcx,[r15+0x280]
40: 48 83 ec 38             sub    rsp,0x38
44: 49 be fe fe fe fe fe    movabs r14,0xfefefefefefefefe
4b: fe fe fe
4e: 41 ff d6                call   r14
51: 48 83 c4 38             add    rsp,0x38
55: c3                      ret";

        public static byte[] BonfireWarp = CommentedAssemblyParser.LoadDefuseOutput(BonfireWarpAssembly);

        private const string BonfireWarpAssembly = @"0:  48 b9 fe fe fe fe fe    movabs rcx,0xfefefefefefefefe
7:  fe fe fe
a:  48 8b 09                mov    rcx,QWORD PTR [rcx]
d:  ba 01 00 00 00          mov    edx,0x1
12: 48 83 ec 38             sub    rsp,0x38
16: 49 be fe fe fe fe fe    movabs r14,0xfefefefefefefefe
1d: fe fe fe
20: 41 ff d6                call   r14
23: 48 83 c4 38             add    rsp,0x38
27: c3                      ret ";
    }
}