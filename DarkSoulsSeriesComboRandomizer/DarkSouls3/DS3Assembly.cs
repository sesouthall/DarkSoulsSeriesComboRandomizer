namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    static class DS3Assembly
    {
        public static byte[] GetItem = CommentedAssemblyParser.LoadDefuseOutput(GetItemAssembly);

        private const string GetItemAssembly = @"0:  48 83 ec 28             sub    rsp,0x28
4:  48 b9 fe fe fe fe fe    movabs rcx,0xfefefefefefefefe
b:  fe fe fe
e:  48 ba fe fe fe fe fe    movabs rdx,0xfefefefefefefefe
15: fe fe fe
18: 49 b8 fe fe fe fe fe    movabs r8,0xfefefefefefefefe
1f: fe fe fe
22: 48 b8 fe fe fe fe fe    movabs rax,0xfefefefefefefefe
29: fe fe fe
2c: ff d0                   call   rax
2e: 48 83 c4 28             add    rsp,0x28
32: c3                      ret";

        public static byte[] BonfireWarp = CommentedAssemblyParser.LoadDefuseOutput(BonfireWarpAssembly);

        private const string BonfireWarpAssembly = @"0:  48 83 ec 48             sub    rsp,0x48
4:  48 b9 fe fe fe fe fe    movabs rcx,0xfefefefefefefefe
b:  fe fe fe
e:  41 b8 fe fe fe fe       mov    r8d,0xfefefefe
14: 45 31 c9                xor    r9d,r9d
17: 48 ba fe fe fe fe fe    movabs rdx,0xfefefefefefefefe
1e: fe fe fe
21: 8d 92 18 fc ff ff       lea    edx,[rdx-0x3e8]
27: 48 b8 fe fe fe fe fe    movabs rax,0xfefefefefefefefe
2e: fe fe fe
31: ff d0                   call   rax
33: 48 83 c4 48             add    rsp,0x48
37: c3                      ret";
    }
}