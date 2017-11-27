
//  Microsoft (R) .NET Framework IL Disassembler.  Version 4.6.1055.0
//  Copyright (c) Microsoft Corporation.  All rights reserved.



// Metadata version: v2.0.50727
.assembly extern System
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly extern mscorlib
{
  .publickeytoken = (B7 7A 5C 56 19 34 E0 89 )                         // .z\V.4..
  .ver 2:0:0:0
}
.assembly Xeora.Web.RegularExpression
{
  .hash algorithm 0x00008004
  .ver 2017:0:6521:0
}
.module Xeora.Web.RegularExpression.dll
// MVID: {48ED9476-1830-4207-9813-CD20E28C6D20}
.imagebase 0x00400000
.file alignment 0x00001000
.stackreserve 0x00100000
.subsystem 0x0003       // WINDOWS_CUI
.corflags 0x00000001    //  ILONLY
// Image base: 0x00E70000


// =============== CLASS MEMBERS DECLARATION ===================

.class private auto ansi Xeora.Web.RegularExpression.MainCapturePatternRunner1
       extends [System]System.Text.RegularExpressions.RegexRunner
{
  .method public virtual instance void  Go() cil managed
  {
    // Code size       10238 (0x27fe)
    .maxstack  337
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32[] V_3,
             int32 V_4,
             int32[] V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10,
             int32 V_11)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextstart
    IL_000e:  stloc.s    V_11
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextbeg
    IL_0016:  stloc.s    V_9
    IL_0018:  ldarg.0
    IL_0019:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_001e:  stloc.s    V_10
    IL_0020:  ldarg.0
    IL_0021:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0026:  stloc.s    V_0
    IL_0028:  ldarg.0
    IL_0029:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_002e:  stloc.s    V_3
    IL_0030:  ldarg.0
    IL_0031:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_0036:  stloc.s    V_2
    IL_0038:  ldarg.0
    IL_0039:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_003e:  stloc.s    V_5
    IL_0040:  ldarg.0
    IL_0041:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0046:  stloc.s    V_4
    IL_0048:  ldloc.s    V_3
    IL_004a:  ldloc.s    V_2
    IL_004c:  ldc.i4.1
    IL_004d:  sub
    IL_004e:  dup
    IL_004f:  stloc.s    V_2
    IL_0051:  ldloc.s    V_0
    IL_0053:  stelem.i4
    IL_0054:  ldloc.s    V_3
    IL_0056:  ldloc.s    V_2
    IL_0058:  ldc.i4.1
    IL_0059:  sub
    IL_005a:  dup
    IL_005b:  stloc.s    V_2
    IL_005d:  ldc.i4.s   0
    IL_005f:  stelem.i4
    IL_0060:  ldloc.s    V_5
    IL_0062:  ldloc.s    V_4
    IL_0064:  ldc.i4.1
    IL_0065:  sub
    IL_0066:  dup
    IL_0067:  stloc.s    V_4
    IL_0069:  ldloc.s    V_0
    IL_006b:  stelem.i4
    IL_006c:  ldloc.s    V_3
    IL_006e:  ldloc.s    V_2
    IL_0070:  ldc.i4.1
    IL_0071:  sub
    IL_0072:  dup
    IL_0073:  stloc.s    V_2
    IL_0075:  ldc.i4.s   1
    IL_0077:  stelem.i4
    IL_0078:  ldloc.s    V_3
    IL_007a:  ldloc.s    V_2
    IL_007c:  ldc.i4.1
    IL_007d:  sub
    IL_007e:  dup
    IL_007f:  stloc.s    V_2
    IL_0081:  ldloc.s    V_0
    IL_0083:  stelem.i4
    IL_0084:  ldloc.s    V_3
    IL_0086:  ldloc.s    V_2
    IL_0088:  ldc.i4.1
    IL_0089:  sub
    IL_008a:  dup
    IL_008b:  stloc.s    V_2
    IL_008d:  ldc.i4.s   2
    IL_008f:  stelem.i4
    IL_0090:  ldloc.s    V_0
    IL_0092:  ldloc.s    V_10
    IL_0094:  bge        IL_1cba

    IL_0099:  ldloc.s    V_1
    IL_009b:  ldloc.s    V_0
    IL_009d:  dup
    IL_009e:  ldc.i4.s   1
    IL_00a0:  add
    IL_00a1:  stloc.s    V_0
    IL_00a3:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00a8:  ldc.i4.s   36
    IL_00aa:  bne.un     IL_1cba

    IL_00af:  ldloc.s    V_5
    IL_00b1:  ldloc.s    V_4
    IL_00b3:  ldc.i4.1
    IL_00b4:  sub
    IL_00b5:  dup
    IL_00b6:  stloc.s    V_4
    IL_00b8:  ldloc.s    V_0
    IL_00ba:  stelem.i4
    IL_00bb:  ldloc.s    V_3
    IL_00bd:  ldloc.s    V_2
    IL_00bf:  ldc.i4.1
    IL_00c0:  sub
    IL_00c1:  dup
    IL_00c2:  stloc.s    V_2
    IL_00c4:  ldc.i4.s   1
    IL_00c6:  stelem.i4
    IL_00c7:  ldloc.s    V_3
    IL_00c9:  ldloc.s    V_2
    IL_00cb:  ldc.i4.1
    IL_00cc:  sub
    IL_00cd:  dup
    IL_00ce:  stloc.s    V_2
    IL_00d0:  ldloc.s    V_0
    IL_00d2:  stelem.i4
    IL_00d3:  ldloc.s    V_3
    IL_00d5:  ldloc.s    V_2
    IL_00d7:  ldc.i4.1
    IL_00d8:  sub
    IL_00d9:  dup
    IL_00da:  stloc.s    V_2
    IL_00dc:  ldc.i4.s   3
    IL_00de:  stelem.i4
    IL_00df:  ldloc.s    V_5
    IL_00e1:  ldloc.s    V_4
    IL_00e3:  ldc.i4.1
    IL_00e4:  sub
    IL_00e5:  dup
    IL_00e6:  stloc.s    V_4
    IL_00e8:  ldloc.s    V_0
    IL_00ea:  stelem.i4
    IL_00eb:  ldloc.s    V_3
    IL_00ed:  ldloc.s    V_2
    IL_00ef:  ldc.i4.1
    IL_00f0:  sub
    IL_00f1:  dup
    IL_00f2:  stloc.s    V_2
    IL_00f4:  ldc.i4.s   1
    IL_00f6:  stelem.i4
    IL_00f7:  ldloc.s    V_3
    IL_00f9:  ldloc.s    V_2
    IL_00fb:  ldc.i4.1
    IL_00fc:  sub
    IL_00fd:  dup
    IL_00fe:  stloc.s    V_2
    IL_0100:  ldloc.s    V_0
    IL_0102:  stelem.i4
    IL_0103:  ldloc.s    V_3
    IL_0105:  ldloc.s    V_2
    IL_0107:  ldc.i4.1
    IL_0108:  sub
    IL_0109:  dup
    IL_010a:  stloc.s    V_2
    IL_010c:  ldc.i4.s   4
    IL_010e:  stelem.i4
    IL_010f:  ldloc.s    V_5
    IL_0111:  ldloc.s    V_4
    IL_0113:  ldc.i4.1
    IL_0114:  sub
    IL_0115:  dup
    IL_0116:  stloc.s    V_4
    IL_0118:  ldc.i4.s   -1
    IL_011a:  stelem.i4
    IL_011b:  ldloc.s    V_5
    IL_011d:  ldloc.s    V_4
    IL_011f:  ldc.i4.1
    IL_0120:  sub
    IL_0121:  dup
    IL_0122:  stloc.s    V_4
    IL_0124:  ldc.i4.s   0
    IL_0126:  stelem.i4
    IL_0127:  ldloc.s    V_3
    IL_0129:  ldloc.s    V_2
    IL_012b:  ldc.i4.1
    IL_012c:  sub
    IL_012d:  dup
    IL_012e:  stloc.s    V_2
    IL_0130:  ldc.i4.s   5
    IL_0132:  stelem.i4
    IL_0133:  br         IL_0260

    IL_0138:  ldloc.s    V_5
    IL_013a:  ldloc.s    V_4
    IL_013c:  ldc.i4.1
    IL_013d:  sub
    IL_013e:  dup
    IL_013f:  stloc.s    V_4
    IL_0141:  ldloc.s    V_0
    IL_0143:  stelem.i4
    IL_0144:  ldloc.s    V_3
    IL_0146:  ldloc.s    V_2
    IL_0148:  ldc.i4.1
    IL_0149:  sub
    IL_014a:  dup
    IL_014b:  stloc.s    V_2
    IL_014d:  ldc.i4.s   1
    IL_014f:  stelem.i4
    IL_0150:  ldloc.s    V_3
    IL_0152:  ldloc.s    V_2
    IL_0154:  ldc.i4.1
    IL_0155:  sub
    IL_0156:  dup
    IL_0157:  stloc.s    V_2
    IL_0159:  ldloc.s    V_0
    IL_015b:  stelem.i4
    IL_015c:  ldloc.s    V_3
    IL_015e:  ldloc.s    V_2
    IL_0160:  ldc.i4.1
    IL_0161:  sub
    IL_0162:  dup
    IL_0163:  stloc.s    V_2
    IL_0165:  ldc.i4.s   6
    IL_0167:  stelem.i4
    IL_0168:  ldc.i4.s   1
    IL_016a:  ldloc.s    V_10
    IL_016c:  ldloc.s    V_0
    IL_016e:  sub
    IL_016f:  bgt        IL_1cba

    IL_0174:  ldloc.s    V_0
    IL_0176:  ldc.i4.s   1
    IL_0178:  add
    IL_0179:  stloc.s    V_0
    IL_017b:  ldc.i4.s   1
    IL_017d:  stloc.s    V_6
    IL_017f:  ldloc.s    V_1
    IL_0181:  ldloc.s    V_0
    IL_0183:  ldloc.s    V_6
    IL_0185:  dup
    IL_0186:  ldc.i4.s   1
    IL_0188:  sub
    IL_0189:  stloc.s    V_6
    IL_018b:  sub
    IL_018c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0191:  ldc.i4.s   35
    IL_0193:  bne.un     IL_1cba

    IL_0198:  ldloc.s    V_6
    IL_019a:  ldc.i4.s   0
    IL_019c:  bgt.s      IL_017f

    IL_019e:  ldloc.s    V_10
    IL_01a0:  ldloc.s    V_0
    IL_01a2:  sub
    IL_01a3:  dup
    IL_01a4:  stloc.s    V_7
    IL_01a6:  ldc.i4.s   1
    IL_01a8:  add
    IL_01a9:  stloc.s    V_6
    IL_01ab:  ldloc.s    V_6
    IL_01ad:  ldc.i4.s   1
    IL_01af:  sub
    IL_01b0:  dup
    IL_01b1:  stloc.s    V_6
    IL_01b3:  ldc.i4.s   0
    IL_01b5:  ble.s      IL_01d1

    IL_01b7:  ldloc.s    V_1
    IL_01b9:  ldloc.s    V_0
    IL_01bb:  dup
    IL_01bc:  ldc.i4.s   1
    IL_01be:  add
    IL_01bf:  stloc.s    V_0
    IL_01c1:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_01c6:  ldc.i4.s   35
    IL_01c8:  beq.s      IL_01ab

    IL_01ca:  ldloc.s    V_0
    IL_01cc:  ldc.i4.s   1
    IL_01ce:  sub
    IL_01cf:  stloc.s    V_0
    IL_01d1:  ldloc.s    V_7
    IL_01d3:  ldloc.s    V_6
    IL_01d5:  ble.s      IL_0204

    IL_01d7:  ldloc.s    V_3
    IL_01d9:  ldloc.s    V_2
    IL_01db:  ldc.i4.1
    IL_01dc:  sub
    IL_01dd:  dup
    IL_01de:  stloc.s    V_2
    IL_01e0:  ldloc.s    V_7
    IL_01e2:  ldloc.s    V_6
    IL_01e4:  sub
    IL_01e5:  ldc.i4.s   1
    IL_01e7:  sub
    IL_01e8:  stelem.i4
    IL_01e9:  ldloc.s    V_3
    IL_01eb:  ldloc.s    V_2
    IL_01ed:  ldc.i4.1
    IL_01ee:  sub
    IL_01ef:  dup
    IL_01f0:  stloc.s    V_2
    IL_01f2:  ldloc.s    V_0
    IL_01f4:  ldc.i4.s   1
    IL_01f6:  sub
    IL_01f7:  stelem.i4
    IL_01f8:  ldloc.s    V_3
    IL_01fa:  ldloc.s    V_2
    IL_01fc:  ldc.i4.1
    IL_01fd:  sub
    IL_01fe:  dup
    IL_01ff:  stloc.s    V_2
    IL_0201:  ldc.i4.s   7
    IL_0203:  stelem.i4
    IL_0204:  br         IL_0230

    IL_0209:  ldloc.s    V_0
    IL_020b:  ldloc.s    V_10
    IL_020d:  bge        IL_1cba

    IL_0212:  ldloc.s    V_1
    IL_0214:  ldloc.s    V_0
    IL_0216:  dup
    IL_0217:  ldc.i4.s   1
    IL_0219:  add
    IL_021a:  stloc.s    V_0
    IL_021c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0221:  ldstr      bytearray (00 00 08 00 00 00 2A 00 2C 00 2D 00 2E 00 5E 00   // ......*.,.-...^.
                                    5F 00 7E 00 7F 00 )                               // _.~...
    IL_0226:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_022b:  brfalse    IL_1cba

    IL_0230:  ldloc.s    V_5
    IL_0232:  ldloc.s    V_4
    IL_0234:  dup
    IL_0235:  ldc.i4.1
    IL_0236:  add
    IL_0237:  stloc.s    V_4
    IL_0239:  ldelem.i4
    IL_023a:  stloc.s    V_6
    IL_023c:  ldarg.0
    IL_023d:  ldc.i4.s   3
    IL_023f:  ldloc.s    V_6
    IL_0241:  ldloc.s    V_0
    IL_0243:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0248:  ldloc.s    V_3
    IL_024a:  ldloc.s    V_2
    IL_024c:  ldc.i4.1
    IL_024d:  sub
    IL_024e:  dup
    IL_024f:  stloc.s    V_2
    IL_0251:  ldloc.s    V_6
    IL_0253:  stelem.i4
    IL_0254:  ldloc.s    V_3
    IL_0256:  ldloc.s    V_2
    IL_0258:  ldc.i4.1
    IL_0259:  sub
    IL_025a:  dup
    IL_025b:  stloc.s    V_2
    IL_025d:  ldc.i4.s   8
    IL_025f:  stelem.i4
    IL_0260:  ldloc.s    V_5
    IL_0262:  ldloc.s    V_4
    IL_0264:  dup
    IL_0265:  ldc.i4.1
    IL_0266:  add
    IL_0267:  stloc.s    V_4
    IL_0269:  ldelem.i4
    IL_026a:  stloc.s    V_6
    IL_026c:  ldloc.s    V_5
    IL_026e:  ldloc.s    V_4
    IL_0270:  dup
    IL_0271:  ldc.i4.1
    IL_0272:  add
    IL_0273:  stloc.s    V_4
    IL_0275:  ldelem.i4
    IL_0276:  dup
    IL_0277:  stloc.s    V_7
    IL_0279:  ldloc.s    V_3
    IL_027b:  ldloc.s    V_2
    IL_027d:  ldc.i4.1
    IL_027e:  sub
    IL_027f:  dup
    IL_0280:  stloc.s    V_2
    IL_0282:  ldloc.s    V_7
    IL_0284:  stelem.i4
    IL_0285:  ldloc.s    V_0
    IL_0287:  bne.un.s   IL_028f

    IL_0289:  ldloc.s    V_6
    IL_028b:  ldc.i4.s   0
    IL_028d:  bge.s      IL_02e2

    IL_028f:  ldloc.s    V_6
    IL_0291:  ldc.i4.s   1
    IL_0293:  bge.s      IL_02e2

    IL_0295:  ldloc.s    V_5
    IL_0297:  ldloc.s    V_4
    IL_0299:  ldc.i4.1
    IL_029a:  sub
    IL_029b:  dup
    IL_029c:  stloc.s    V_4
    IL_029e:  ldloc.s    V_0
    IL_02a0:  stelem.i4
    IL_02a1:  ldloc.s    V_5
    IL_02a3:  ldloc.s    V_4
    IL_02a5:  ldc.i4.1
    IL_02a6:  sub
    IL_02a7:  dup
    IL_02a8:  stloc.s    V_4
    IL_02aa:  ldloc.s    V_6
    IL_02ac:  ldc.i4.s   1
    IL_02ae:  add
    IL_02af:  stelem.i4
    IL_02b0:  ldloc.s    V_3
    IL_02b2:  ldloc.s    V_2
    IL_02b4:  ldc.i4.1
    IL_02b5:  sub
    IL_02b6:  dup
    IL_02b7:  stloc.s    V_2
    IL_02b9:  ldc.i4.s   9
    IL_02bb:  stelem.i4
    IL_02bc:  ldloc.s    V_2
    IL_02be:  ldc.i4     0x188
    IL_02c3:  ble.s      IL_02d1

    IL_02c5:  ldloc.s    V_4
    IL_02c7:  ldc.i4     0x126
    IL_02cc:  bgt        IL_0138

    IL_02d1:  ldloc.s    V_3
    IL_02d3:  ldloc.s    V_2
    IL_02d5:  ldc.i4.1
    IL_02d6:  sub
    IL_02d7:  dup
    IL_02d8:  stloc.s    V_2
    IL_02da:  ldc.i4.s   10
    IL_02dc:  stelem.i4
    IL_02dd:  br         IL_1cba

    IL_02e2:  ldloc.s    V_3
    IL_02e4:  ldloc.s    V_2
    IL_02e6:  ldc.i4.1
    IL_02e7:  sub
    IL_02e8:  dup
    IL_02e9:  stloc.s    V_2
    IL_02eb:  ldloc.s    V_6
    IL_02ed:  stelem.i4
    IL_02ee:  ldloc.s    V_3
    IL_02f0:  ldloc.s    V_2
    IL_02f2:  ldc.i4.1
    IL_02f3:  sub
    IL_02f4:  dup
    IL_02f5:  stloc.s    V_2
    IL_02f7:  ldc.i4.s   11
    IL_02f9:  stelem.i4
    IL_02fa:  ldloc.s    V_5
    IL_02fc:  ldloc.s    V_4
    IL_02fe:  ldc.i4.1
    IL_02ff:  sub
    IL_0300:  dup
    IL_0301:  stloc.s    V_4
    IL_0303:  ldloc.s    V_0
    IL_0305:  stelem.i4
    IL_0306:  ldloc.s    V_3
    IL_0308:  ldloc.s    V_2
    IL_030a:  ldc.i4.1
    IL_030b:  sub
    IL_030c:  dup
    IL_030d:  stloc.s    V_2
    IL_030f:  ldc.i4.s   1
    IL_0311:  stelem.i4
    IL_0312:  ldc.i4.s   1
    IL_0314:  ldloc.s    V_10
    IL_0316:  ldloc.s    V_0
    IL_0318:  sub
    IL_0319:  bgt        IL_1cba

    IL_031e:  ldloc.s    V_0
    IL_0320:  ldc.i4.s   1
    IL_0322:  add
    IL_0323:  stloc.s    V_0
    IL_0325:  ldc.i4.s   1
    IL_0327:  stloc.s    V_6
    IL_0329:  ldloc.s    V_1
    IL_032b:  ldloc.s    V_0
    IL_032d:  ldloc.s    V_6
    IL_032f:  dup
    IL_0330:  ldc.i4.s   1
    IL_0332:  sub
    IL_0333:  stloc.s    V_6
    IL_0335:  sub
    IL_0336:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_033b:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_0340:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0345:  brfalse    IL_1cba

    IL_034a:  ldloc.s    V_6
    IL_034c:  ldc.i4.s   0
    IL_034e:  bgt        IL_0329

    IL_0353:  ldloc.s    V_10
    IL_0355:  ldloc.s    V_0
    IL_0357:  sub
    IL_0358:  dup
    IL_0359:  stloc.s    V_7
    IL_035b:  ldc.i4.s   1
    IL_035d:  add
    IL_035e:  stloc.s    V_6
    IL_0360:  ldloc.s    V_6
    IL_0362:  ldc.i4.s   1
    IL_0364:  sub
    IL_0365:  dup
    IL_0366:  stloc.s    V_6
    IL_0368:  ldc.i4.s   0
    IL_036a:  ble        IL_0394

    IL_036f:  ldloc.s    V_1
    IL_0371:  ldloc.s    V_0
    IL_0373:  dup
    IL_0374:  ldc.i4.s   1
    IL_0376:  add
    IL_0377:  stloc.s    V_0
    IL_0379:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_037e:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_0383:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0388:  brtrue     IL_0360

    IL_038d:  ldloc.s    V_0
    IL_038f:  ldc.i4.s   1
    IL_0391:  sub
    IL_0392:  stloc.s    V_0
    IL_0394:  ldloc.s    V_7
    IL_0396:  ldloc.s    V_6
    IL_0398:  ble.s      IL_03c7

    IL_039a:  ldloc.s    V_3
    IL_039c:  ldloc.s    V_2
    IL_039e:  ldc.i4.1
    IL_039f:  sub
    IL_03a0:  dup
    IL_03a1:  stloc.s    V_2
    IL_03a3:  ldloc.s    V_7
    IL_03a5:  ldloc.s    V_6
    IL_03a7:  sub
    IL_03a8:  ldc.i4.s   1
    IL_03aa:  sub
    IL_03ab:  stelem.i4
    IL_03ac:  ldloc.s    V_3
    IL_03ae:  ldloc.s    V_2
    IL_03b0:  ldc.i4.1
    IL_03b1:  sub
    IL_03b2:  dup
    IL_03b3:  stloc.s    V_2
    IL_03b5:  ldloc.s    V_0
    IL_03b7:  ldc.i4.s   1
    IL_03b9:  sub
    IL_03ba:  stelem.i4
    IL_03bb:  ldloc.s    V_3
    IL_03bd:  ldloc.s    V_2
    IL_03bf:  ldc.i4.1
    IL_03c0:  sub
    IL_03c1:  dup
    IL_03c2:  stloc.s    V_2
    IL_03c4:  ldc.i4.s   12
    IL_03c6:  stelem.i4
    IL_03c7:  ldloc.s    V_5
    IL_03c9:  ldloc.s    V_4
    IL_03cb:  dup
    IL_03cc:  ldc.i4.1
    IL_03cd:  add
    IL_03ce:  stloc.s    V_4
    IL_03d0:  ldelem.i4
    IL_03d1:  stloc.s    V_6
    IL_03d3:  ldarg.0
    IL_03d4:  ldc.i4.s   4
    IL_03d6:  ldloc.s    V_6
    IL_03d8:  ldloc.s    V_0
    IL_03da:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_03df:  ldloc.s    V_3
    IL_03e1:  ldloc.s    V_2
    IL_03e3:  ldc.i4.1
    IL_03e4:  sub
    IL_03e5:  dup
    IL_03e6:  stloc.s    V_2
    IL_03e8:  ldloc.s    V_6
    IL_03ea:  stelem.i4
    IL_03eb:  ldloc.s    V_3
    IL_03ed:  ldloc.s    V_2
    IL_03ef:  ldc.i4.1
    IL_03f0:  sub
    IL_03f1:  dup
    IL_03f2:  stloc.s    V_2
    IL_03f4:  ldc.i4.s   8
    IL_03f6:  stelem.i4
    IL_03f7:  br         IL_0660

    IL_03fc:  ldloc.s    V_0
    IL_03fe:  ldloc.s    V_10
    IL_0400:  bge        IL_1cba

    IL_0405:  ldloc.s    V_1
    IL_0407:  ldloc.s    V_0
    IL_0409:  dup
    IL_040a:  ldc.i4.s   1
    IL_040c:  add
    IL_040d:  stloc.s    V_0
    IL_040f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0414:  ldc.i4.s   64
    IL_0416:  bne.un     IL_1cba

    IL_041b:  ldloc.s    V_10
    IL_041d:  ldloc.s    V_0
    IL_041f:  sub
    IL_0420:  dup
    IL_0421:  stloc.s    V_7
    IL_0423:  ldc.i4.s   1
    IL_0425:  add
    IL_0426:  stloc.s    V_6
    IL_0428:  ldloc.s    V_6
    IL_042a:  ldc.i4.s   1
    IL_042c:  sub
    IL_042d:  dup
    IL_042e:  stloc.s    V_6
    IL_0430:  ldc.i4.s   0
    IL_0432:  ble        IL_045c

    IL_0437:  ldloc.s    V_1
    IL_0439:  ldloc.s    V_0
    IL_043b:  dup
    IL_043c:  ldc.i4.s   1
    IL_043e:  add
    IL_043f:  stloc.s    V_0
    IL_0441:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0446:  ldstr      bytearray (00 00 04 00 00 00 23 00 24 00 2D 00 2E 00 )       // ......#.$.-...
    IL_044b:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0450:  brtrue     IL_0428

    IL_0455:  ldloc.s    V_0
    IL_0457:  ldc.i4.s   1
    IL_0459:  sub
    IL_045a:  stloc.s    V_0
    IL_045c:  ldloc.s    V_7
    IL_045e:  ldloc.s    V_6
    IL_0460:  ble.s      IL_048f

    IL_0462:  ldloc.s    V_3
    IL_0464:  ldloc.s    V_2
    IL_0466:  ldc.i4.1
    IL_0467:  sub
    IL_0468:  dup
    IL_0469:  stloc.s    V_2
    IL_046b:  ldloc.s    V_7
    IL_046d:  ldloc.s    V_6
    IL_046f:  sub
    IL_0470:  ldc.i4.s   1
    IL_0472:  sub
    IL_0473:  stelem.i4
    IL_0474:  ldloc.s    V_3
    IL_0476:  ldloc.s    V_2
    IL_0478:  ldc.i4.1
    IL_0479:  sub
    IL_047a:  dup
    IL_047b:  stloc.s    V_2
    IL_047d:  ldloc.s    V_0
    IL_047f:  ldc.i4.s   1
    IL_0481:  sub
    IL_0482:  stelem.i4
    IL_0483:  ldloc.s    V_3
    IL_0485:  ldloc.s    V_2
    IL_0487:  ldc.i4.1
    IL_0488:  sub
    IL_0489:  dup
    IL_048a:  stloc.s    V_2
    IL_048c:  ldc.i4.s   13
    IL_048e:  stelem.i4
    IL_048f:  ldloc.s    V_5
    IL_0491:  ldloc.s    V_4
    IL_0493:  ldc.i4.1
    IL_0494:  sub
    IL_0495:  dup
    IL_0496:  stloc.s    V_4
    IL_0498:  ldloc.s    V_0
    IL_049a:  stelem.i4
    IL_049b:  ldloc.s    V_3
    IL_049d:  ldloc.s    V_2
    IL_049f:  ldc.i4.1
    IL_04a0:  sub
    IL_04a1:  dup
    IL_04a2:  stloc.s    V_2
    IL_04a4:  ldc.i4.s   1
    IL_04a6:  stelem.i4
    IL_04a7:  ldc.i4.s   1
    IL_04a9:  ldloc.s    V_10
    IL_04ab:  ldloc.s    V_0
    IL_04ad:  sub
    IL_04ae:  bgt        IL_1cba

    IL_04b3:  ldloc.s    V_0
    IL_04b5:  ldc.i4.s   1
    IL_04b7:  add
    IL_04b8:  stloc.s    V_0
    IL_04ba:  ldc.i4.s   1
    IL_04bc:  stloc.s    V_6
    IL_04be:  ldloc.s    V_1
    IL_04c0:  ldloc.s    V_0
    IL_04c2:  ldloc.s    V_6
    IL_04c4:  dup
    IL_04c5:  ldc.i4.s   1
    IL_04c7:  sub
    IL_04c8:  stloc.s    V_6
    IL_04ca:  sub
    IL_04cb:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_04d0:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_04d5:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_04da:  brfalse    IL_1cba

    IL_04df:  ldloc.s    V_6
    IL_04e1:  ldc.i4.s   0
    IL_04e3:  bgt        IL_04be

    IL_04e8:  ldloc.s    V_10
    IL_04ea:  ldloc.s    V_0
    IL_04ec:  sub
    IL_04ed:  dup
    IL_04ee:  stloc.s    V_7
    IL_04f0:  ldc.i4.s   1
    IL_04f2:  add
    IL_04f3:  stloc.s    V_6
    IL_04f5:  ldloc.s    V_6
    IL_04f7:  ldc.i4.s   1
    IL_04f9:  sub
    IL_04fa:  dup
    IL_04fb:  stloc.s    V_6
    IL_04fd:  ldc.i4.s   0
    IL_04ff:  ble        IL_0529

    IL_0504:  ldloc.s    V_1
    IL_0506:  ldloc.s    V_0
    IL_0508:  dup
    IL_0509:  ldc.i4.s   1
    IL_050b:  add
    IL_050c:  stloc.s    V_0
    IL_050e:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0513:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_0518:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_051d:  brtrue     IL_04f5

    IL_0522:  ldloc.s    V_0
    IL_0524:  ldc.i4.s   1
    IL_0526:  sub
    IL_0527:  stloc.s    V_0
    IL_0529:  ldloc.s    V_7
    IL_052b:  ldloc.s    V_6
    IL_052d:  ble.s      IL_055c

    IL_052f:  ldloc.s    V_3
    IL_0531:  ldloc.s    V_2
    IL_0533:  ldc.i4.1
    IL_0534:  sub
    IL_0535:  dup
    IL_0536:  stloc.s    V_2
    IL_0538:  ldloc.s    V_7
    IL_053a:  ldloc.s    V_6
    IL_053c:  sub
    IL_053d:  ldc.i4.s   1
    IL_053f:  sub
    IL_0540:  stelem.i4
    IL_0541:  ldloc.s    V_3
    IL_0543:  ldloc.s    V_2
    IL_0545:  ldc.i4.1
    IL_0546:  sub
    IL_0547:  dup
    IL_0548:  stloc.s    V_2
    IL_054a:  ldloc.s    V_0
    IL_054c:  ldc.i4.s   1
    IL_054e:  sub
    IL_054f:  stelem.i4
    IL_0550:  ldloc.s    V_3
    IL_0552:  ldloc.s    V_2
    IL_0554:  ldc.i4.1
    IL_0555:  sub
    IL_0556:  dup
    IL_0557:  stloc.s    V_2
    IL_0559:  ldc.i4.s   14
    IL_055b:  stelem.i4
    IL_055c:  ldloc.s    V_0
    IL_055e:  ldloc.s    V_10
    IL_0560:  bge        IL_1cba

    IL_0565:  ldloc.s    V_1
    IL_0567:  ldloc.s    V_0
    IL_0569:  dup
    IL_056a:  ldc.i4.s   1
    IL_056c:  add
    IL_056d:  stloc.s    V_0
    IL_056f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0574:  ldc.i4.s   46
    IL_0576:  bne.un     IL_1cba

    IL_057b:  ldloc.s    V_5
    IL_057d:  ldloc.s    V_4
    IL_057f:  dup
    IL_0580:  ldc.i4.1
    IL_0581:  add
    IL_0582:  stloc.s    V_4
    IL_0584:  ldelem.i4
    IL_0585:  stloc.s    V_6
    IL_0587:  ldarg.0
    IL_0588:  ldc.i4.s   5
    IL_058a:  ldloc.s    V_6
    IL_058c:  ldloc.s    V_0
    IL_058e:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0593:  ldloc.s    V_3
    IL_0595:  ldloc.s    V_2
    IL_0597:  ldc.i4.1
    IL_0598:  sub
    IL_0599:  dup
    IL_059a:  stloc.s    V_2
    IL_059c:  ldloc.s    V_6
    IL_059e:  stelem.i4
    IL_059f:  ldloc.s    V_3
    IL_05a1:  ldloc.s    V_2
    IL_05a3:  ldc.i4.1
    IL_05a4:  sub
    IL_05a5:  dup
    IL_05a6:  stloc.s    V_2
    IL_05a8:  ldc.i4.s   8
    IL_05aa:  stelem.i4
    IL_05ab:  ldc.i4.s   1
    IL_05ad:  ldloc.s    V_10
    IL_05af:  ldloc.s    V_0
    IL_05b1:  sub
    IL_05b2:  bgt        IL_1cba

    IL_05b7:  ldloc.s    V_0
    IL_05b9:  ldc.i4.s   1
    IL_05bb:  add
    IL_05bc:  stloc.s    V_0
    IL_05be:  ldc.i4.s   1
    IL_05c0:  stloc.s    V_6
    IL_05c2:  ldloc.s    V_1
    IL_05c4:  ldloc.s    V_0
    IL_05c6:  ldloc.s    V_6
    IL_05c8:  dup
    IL_05c9:  ldc.i4.s   1
    IL_05cb:  sub
    IL_05cc:  stloc.s    V_6
    IL_05ce:  sub
    IL_05cf:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_05d4:  ldstr      bytearray (00 00 04 00 09 00 2B 00 2C 00 2E 00 2F 00 00 00   // ......+.,.../...
                                    02 00 04 00 05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_05d9:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_05de:  brfalse    IL_1cba

    IL_05e3:  ldloc.s    V_6
    IL_05e5:  ldc.i4.s   0
    IL_05e7:  bgt        IL_05c2

    IL_05ec:  ldloc.s    V_10
    IL_05ee:  ldloc.s    V_0
    IL_05f0:  sub
    IL_05f1:  dup
    IL_05f2:  stloc.s    V_7
    IL_05f4:  ldc.i4.s   1
    IL_05f6:  add
    IL_05f7:  stloc.s    V_6
    IL_05f9:  ldloc.s    V_6
    IL_05fb:  ldc.i4.s   1
    IL_05fd:  sub
    IL_05fe:  dup
    IL_05ff:  stloc.s    V_6
    IL_0601:  ldc.i4.s   0
    IL_0603:  ble        IL_062d

    IL_0608:  ldloc.s    V_1
    IL_060a:  ldloc.s    V_0
    IL_060c:  dup
    IL_060d:  ldc.i4.s   1
    IL_060f:  add
    IL_0610:  stloc.s    V_0
    IL_0612:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0617:  ldstr      bytearray (00 00 04 00 09 00 2B 00 2C 00 2E 00 2F 00 00 00   // ......+.,.../...
                                    02 00 04 00 05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_061c:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0621:  brtrue     IL_05f9

    IL_0626:  ldloc.s    V_0
    IL_0628:  ldc.i4.s   1
    IL_062a:  sub
    IL_062b:  stloc.s    V_0
    IL_062d:  ldloc.s    V_7
    IL_062f:  ldloc.s    V_6
    IL_0631:  ble.s      IL_0660

    IL_0633:  ldloc.s    V_3
    IL_0635:  ldloc.s    V_2
    IL_0637:  ldc.i4.1
    IL_0638:  sub
    IL_0639:  dup
    IL_063a:  stloc.s    V_2
    IL_063c:  ldloc.s    V_7
    IL_063e:  ldloc.s    V_6
    IL_0640:  sub
    IL_0641:  ldc.i4.s   1
    IL_0643:  sub
    IL_0644:  stelem.i4
    IL_0645:  ldloc.s    V_3
    IL_0647:  ldloc.s    V_2
    IL_0649:  ldc.i4.1
    IL_064a:  sub
    IL_064b:  dup
    IL_064c:  stloc.s    V_2
    IL_064e:  ldloc.s    V_0
    IL_0650:  ldc.i4.s   1
    IL_0652:  sub
    IL_0653:  stelem.i4
    IL_0654:  ldloc.s    V_3
    IL_0656:  ldloc.s    V_2
    IL_0658:  ldc.i4.1
    IL_0659:  sub
    IL_065a:  dup
    IL_065b:  stloc.s    V_2
    IL_065d:  ldc.i4.s   15
    IL_065f:  stelem.i4
    IL_0660:  ldloc.s    V_5
    IL_0662:  ldloc.s    V_4
    IL_0664:  dup
    IL_0665:  ldc.i4.1
    IL_0666:  add
    IL_0667:  stloc.s    V_4
    IL_0669:  ldelem.i4
    IL_066a:  stloc.s    V_6
    IL_066c:  ldarg.0
    IL_066d:  ldc.i4.s   2
    IL_066f:  ldloc.s    V_6
    IL_0671:  ldloc.s    V_0
    IL_0673:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0678:  ldloc.s    V_3
    IL_067a:  ldloc.s    V_2
    IL_067c:  ldc.i4.1
    IL_067d:  sub
    IL_067e:  dup
    IL_067f:  stloc.s    V_2
    IL_0681:  ldloc.s    V_6
    IL_0683:  stelem.i4
    IL_0684:  ldloc.s    V_3
    IL_0686:  ldloc.s    V_2
    IL_0688:  ldc.i4.1
    IL_0689:  sub
    IL_068a:  dup
    IL_068b:  stloc.s    V_2
    IL_068d:  ldc.i4.s   8
    IL_068f:  stelem.i4
    IL_0690:  ldloc.s    V_0
    IL_0692:  ldloc.s    V_10
    IL_0694:  bge        IL_1cba

    IL_0699:  ldloc.s    V_1
    IL_069b:  ldloc.s    V_0
    IL_069d:  dup
    IL_069e:  ldc.i4.s   1
    IL_06a0:  add
    IL_06a1:  stloc.s    V_0
    IL_06a3:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_06a8:  ldc.i4.s   36
    IL_06aa:  bne.un     IL_1cba

    IL_06af:  br         IL_1a04

    IL_06b4:  ldloc.s    V_3
    IL_06b6:  ldloc.s    V_2
    IL_06b8:  ldc.i4.1
    IL_06b9:  sub
    IL_06ba:  dup
    IL_06bb:  stloc.s    V_2
    IL_06bd:  ldloc.s    V_0
    IL_06bf:  stelem.i4
    IL_06c0:  ldloc.s    V_3
    IL_06c2:  ldloc.s    V_2
    IL_06c4:  ldc.i4.1
    IL_06c5:  sub
    IL_06c6:  dup
    IL_06c7:  stloc.s    V_2
    IL_06c9:  ldc.i4.s   16
    IL_06cb:  stelem.i4
    IL_06cc:  ldc.i4.s   1
    IL_06ce:  ldloc.s    V_10
    IL_06d0:  ldloc.s    V_0
    IL_06d2:  sub
    IL_06d3:  bgt        IL_1cba

    IL_06d8:  ldloc.s    V_0
    IL_06da:  ldc.i4.s   1
    IL_06dc:  add
    IL_06dd:  stloc.s    V_0
    IL_06df:  ldc.i4.s   1
    IL_06e1:  stloc.s    V_6
    IL_06e3:  ldloc.s    V_1
    IL_06e5:  ldloc.s    V_0
    IL_06e7:  ldloc.s    V_6
    IL_06e9:  dup
    IL_06ea:  ldc.i4.s   1
    IL_06ec:  sub
    IL_06ed:  stloc.s    V_6
    IL_06ef:  sub
    IL_06f0:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_06f5:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_06fa:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_06ff:  brfalse    IL_1cba

    IL_0704:  ldloc.s    V_6
    IL_0706:  ldc.i4.s   0
    IL_0708:  bgt        IL_06e3

    IL_070d:  ldloc.s    V_10
    IL_070f:  ldloc.s    V_0
    IL_0711:  sub
    IL_0712:  dup
    IL_0713:  stloc.s    V_7
    IL_0715:  ldc.i4.s   1
    IL_0717:  add
    IL_0718:  stloc.s    V_6
    IL_071a:  ldloc.s    V_6
    IL_071c:  ldc.i4.s   1
    IL_071e:  sub
    IL_071f:  dup
    IL_0720:  stloc.s    V_6
    IL_0722:  ldc.i4.s   0
    IL_0724:  ble        IL_074e

    IL_0729:  ldloc.s    V_1
    IL_072b:  ldloc.s    V_0
    IL_072d:  dup
    IL_072e:  ldc.i4.s   1
    IL_0730:  add
    IL_0731:  stloc.s    V_0
    IL_0733:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0738:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_073d:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0742:  brtrue     IL_071a

    IL_0747:  ldloc.s    V_0
    IL_0749:  ldc.i4.s   1
    IL_074b:  sub
    IL_074c:  stloc.s    V_0
    IL_074e:  ldloc.s    V_7
    IL_0750:  ldloc.s    V_6
    IL_0752:  ble.s      IL_0781

    IL_0754:  ldloc.s    V_3
    IL_0756:  ldloc.s    V_2
    IL_0758:  ldc.i4.1
    IL_0759:  sub
    IL_075a:  dup
    IL_075b:  stloc.s    V_2
    IL_075d:  ldloc.s    V_7
    IL_075f:  ldloc.s    V_6
    IL_0761:  sub
    IL_0762:  ldc.i4.s   1
    IL_0764:  sub
    IL_0765:  stelem.i4
    IL_0766:  ldloc.s    V_3
    IL_0768:  ldloc.s    V_2
    IL_076a:  ldc.i4.1
    IL_076b:  sub
    IL_076c:  dup
    IL_076d:  stloc.s    V_2
    IL_076f:  ldloc.s    V_0
    IL_0771:  ldc.i4.s   1
    IL_0773:  sub
    IL_0774:  stelem.i4
    IL_0775:  ldloc.s    V_3
    IL_0777:  ldloc.s    V_2
    IL_0779:  ldc.i4.1
    IL_077a:  sub
    IL_077b:  dup
    IL_077c:  stloc.s    V_2
    IL_077e:  ldc.i4.s   17
    IL_0780:  stelem.i4
    IL_0781:  ldc.i4.s   2
    IL_0783:  ldloc.s    V_10
    IL_0785:  ldloc.s    V_0
    IL_0787:  sub
    IL_0788:  bgt        IL_1cba

    IL_078d:  ldloc.s    V_1
    IL_078f:  ldloc.s    V_0
    IL_0791:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0796:  ldc.i4.s   58
    IL_0798:  bne.un     IL_1cba

    IL_079d:  ldloc.s    V_1
    IL_079f:  ldloc.s    V_0
    IL_07a1:  ldc.i4.s   1
    IL_07a3:  add
    IL_07a4:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_07a9:  ldc.i4.s   123
    IL_07ab:  bne.un     IL_1cba

    IL_07b0:  ldloc.s    V_0
    IL_07b2:  ldc.i4.s   2
    IL_07b4:  add
    IL_07b5:  stloc.s    V_0
    IL_07b7:  br         IL_1a04

    IL_07bc:  ldloc.s    V_0
    IL_07be:  ldloc.s    V_10
    IL_07c0:  bge        IL_1cba

    IL_07c5:  ldloc.s    V_1
    IL_07c7:  ldloc.s    V_0
    IL_07c9:  dup
    IL_07ca:  ldc.i4.s   1
    IL_07cc:  add
    IL_07cd:  stloc.s    V_0
    IL_07cf:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_07d4:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_07d9:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_07de:  brfalse    IL_1cba

    IL_07e3:  ldloc.s    V_5
    IL_07e5:  ldloc.s    V_4
    IL_07e7:  ldc.i4.1
    IL_07e8:  sub
    IL_07e9:  dup
    IL_07ea:  stloc.s    V_4
    IL_07ec:  ldc.i4.s   -1
    IL_07ee:  stelem.i4
    IL_07ef:  ldloc.s    V_5
    IL_07f1:  ldloc.s    V_4
    IL_07f3:  ldc.i4.1
    IL_07f4:  sub
    IL_07f5:  dup
    IL_07f6:  stloc.s    V_4
    IL_07f8:  ldc.i4.s   0
    IL_07fa:  stelem.i4
    IL_07fb:  ldloc.s    V_3
    IL_07fd:  ldloc.s    V_2
    IL_07ff:  ldc.i4.1
    IL_0800:  sub
    IL_0801:  dup
    IL_0802:  stloc.s    V_2
    IL_0804:  ldc.i4.s   5
    IL_0806:  stelem.i4
    IL_0807:  br         IL_0a52

    IL_080c:  ldloc.s    V_5
    IL_080e:  ldloc.s    V_4
    IL_0810:  ldc.i4.1
    IL_0811:  sub
    IL_0812:  dup
    IL_0813:  stloc.s    V_4
    IL_0815:  ldloc.s    V_0
    IL_0817:  stelem.i4
    IL_0818:  ldloc.s    V_3
    IL_081a:  ldloc.s    V_2
    IL_081c:  ldc.i4.1
    IL_081d:  sub
    IL_081e:  dup
    IL_081f:  stloc.s    V_2
    IL_0821:  ldc.i4.s   1
    IL_0823:  stelem.i4
    IL_0824:  ldloc.s    V_0
    IL_0826:  ldloc.s    V_10
    IL_0828:  bge        IL_1cba

    IL_082d:  ldloc.s    V_1
    IL_082f:  ldloc.s    V_0
    IL_0831:  dup
    IL_0832:  ldc.i4.s   1
    IL_0834:  add
    IL_0835:  stloc.s    V_0
    IL_0837:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_083c:  ldc.i4.s   35
    IL_083e:  bne.un     IL_1cba

    IL_0843:  ldc.i4.s   1
    IL_0845:  ldloc.s    V_10
    IL_0847:  ldloc.s    V_0
    IL_0849:  sub
    IL_084a:  bgt        IL_1cba

    IL_084f:  ldloc.s    V_0
    IL_0851:  ldc.i4.s   1
    IL_0853:  add
    IL_0854:  stloc.s    V_0
    IL_0856:  ldc.i4.s   1
    IL_0858:  stloc.s    V_6
    IL_085a:  ldloc.s    V_1
    IL_085c:  ldloc.s    V_0
    IL_085e:  ldloc.s    V_6
    IL_0860:  dup
    IL_0861:  ldc.i4.s   1
    IL_0863:  sub
    IL_0864:  stloc.s    V_6
    IL_0866:  sub
    IL_0867:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_086c:  ldstr      bytearray (00 00 00 00 01 00 09 00 ) 
    IL_0871:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0876:  brfalse    IL_1cba

    IL_087b:  ldloc.s    V_6
    IL_087d:  ldc.i4.s   0
    IL_087f:  bgt        IL_085a

    IL_0884:  ldloc.s    V_10
    IL_0886:  ldloc.s    V_0
    IL_0888:  sub
    IL_0889:  dup
    IL_088a:  stloc.s    V_7
    IL_088c:  ldc.i4.s   1
    IL_088e:  add
    IL_088f:  stloc.s    V_6
    IL_0891:  ldloc.s    V_6
    IL_0893:  ldc.i4.s   1
    IL_0895:  sub
    IL_0896:  dup
    IL_0897:  stloc.s    V_6
    IL_0899:  ldc.i4.s   0
    IL_089b:  ble        IL_08c5

    IL_08a0:  ldloc.s    V_1
    IL_08a2:  ldloc.s    V_0
    IL_08a4:  dup
    IL_08a5:  ldc.i4.s   1
    IL_08a7:  add
    IL_08a8:  stloc.s    V_0
    IL_08aa:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_08af:  ldstr      bytearray (00 00 00 00 01 00 09 00 ) 
    IL_08b4:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_08b9:  brtrue     IL_0891

    IL_08be:  ldloc.s    V_0
    IL_08c0:  ldc.i4.s   1
    IL_08c2:  sub
    IL_08c3:  stloc.s    V_0
    IL_08c5:  ldloc.s    V_7
    IL_08c7:  ldloc.s    V_6
    IL_08c9:  ble.s      IL_08f8

    IL_08cb:  ldloc.s    V_3
    IL_08cd:  ldloc.s    V_2
    IL_08cf:  ldc.i4.1
    IL_08d0:  sub
    IL_08d1:  dup
    IL_08d2:  stloc.s    V_2
    IL_08d4:  ldloc.s    V_7
    IL_08d6:  ldloc.s    V_6
    IL_08d8:  sub
    IL_08d9:  ldc.i4.s   1
    IL_08db:  sub
    IL_08dc:  stelem.i4
    IL_08dd:  ldloc.s    V_3
    IL_08df:  ldloc.s    V_2
    IL_08e1:  ldc.i4.1
    IL_08e2:  sub
    IL_08e3:  dup
    IL_08e4:  stloc.s    V_2
    IL_08e6:  ldloc.s    V_0
    IL_08e8:  ldc.i4.s   1
    IL_08ea:  sub
    IL_08eb:  stelem.i4
    IL_08ec:  ldloc.s    V_3
    IL_08ee:  ldloc.s    V_2
    IL_08f0:  ldc.i4.1
    IL_08f1:  sub
    IL_08f2:  dup
    IL_08f3:  stloc.s    V_2
    IL_08f5:  ldc.i4.s   18
    IL_08f7:  stelem.i4
    IL_08f8:  ldloc.s    V_5
    IL_08fa:  ldloc.s    V_4
    IL_08fc:  ldc.i4.1
    IL_08fd:  sub
    IL_08fe:  dup
    IL_08ff:  stloc.s    V_4
    IL_0901:  ldc.i4.s   -1
    IL_0903:  stelem.i4
    IL_0904:  ldloc.s    V_5
    IL_0906:  ldloc.s    V_4
    IL_0908:  ldc.i4.1
    IL_0909:  sub
    IL_090a:  dup
    IL_090b:  stloc.s    V_4
    IL_090d:  ldc.i4.s   0
    IL_090f:  stelem.i4
    IL_0910:  ldloc.s    V_3
    IL_0912:  ldloc.s    V_2
    IL_0914:  ldc.i4.1
    IL_0915:  sub
    IL_0916:  dup
    IL_0917:  stloc.s    V_2
    IL_0919:  ldc.i4.s   5
    IL_091b:  stelem.i4
    IL_091c:  br         IL_0988

    IL_0921:  ldloc.s    V_5
    IL_0923:  ldloc.s    V_4
    IL_0925:  ldc.i4.1
    IL_0926:  sub
    IL_0927:  dup
    IL_0928:  stloc.s    V_4
    IL_092a:  ldloc.s    V_0
    IL_092c:  stelem.i4
    IL_092d:  ldloc.s    V_3
    IL_092f:  ldloc.s    V_2
    IL_0931:  ldc.i4.1
    IL_0932:  sub
    IL_0933:  dup
    IL_0934:  stloc.s    V_2
    IL_0936:  ldc.i4.s   1
    IL_0938:  stelem.i4
    IL_0939:  ldloc.s    V_0
    IL_093b:  ldloc.s    V_10
    IL_093d:  bge        IL_1cba

    IL_0942:  ldloc.s    V_1
    IL_0944:  ldloc.s    V_0
    IL_0946:  dup
    IL_0947:  ldc.i4.s   1
    IL_0949:  add
    IL_094a:  stloc.s    V_0
    IL_094c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0951:  ldc.i4.s   43
    IL_0953:  bne.un     IL_1cba

    IL_0958:  ldloc.s    V_5
    IL_095a:  ldloc.s    V_4
    IL_095c:  dup
    IL_095d:  ldc.i4.1
    IL_095e:  add
    IL_095f:  stloc.s    V_4
    IL_0961:  ldelem.i4
    IL_0962:  stloc.s    V_6
    IL_0964:  ldarg.0
    IL_0965:  ldc.i4.s   7
    IL_0967:  ldloc.s    V_6
    IL_0969:  ldloc.s    V_0
    IL_096b:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0970:  ldloc.s    V_3
    IL_0972:  ldloc.s    V_2
    IL_0974:  ldc.i4.1
    IL_0975:  sub
    IL_0976:  dup
    IL_0977:  stloc.s    V_2
    IL_0979:  ldloc.s    V_6
    IL_097b:  stelem.i4
    IL_097c:  ldloc.s    V_3
    IL_097e:  ldloc.s    V_2
    IL_0980:  ldc.i4.1
    IL_0981:  sub
    IL_0982:  dup
    IL_0983:  stloc.s    V_2
    IL_0985:  ldc.i4.s   8
    IL_0987:  stelem.i4
    IL_0988:  ldloc.s    V_5
    IL_098a:  ldloc.s    V_4
    IL_098c:  dup
    IL_098d:  ldc.i4.1
    IL_098e:  add
    IL_098f:  stloc.s    V_4
    IL_0991:  ldelem.i4
    IL_0992:  stloc.s    V_6
    IL_0994:  ldloc.s    V_5
    IL_0996:  ldloc.s    V_4
    IL_0998:  dup
    IL_0999:  ldc.i4.1
    IL_099a:  add
    IL_099b:  stloc.s    V_4
    IL_099d:  ldelem.i4
    IL_099e:  dup
    IL_099f:  stloc.s    V_7
    IL_09a1:  ldloc.s    V_3
    IL_09a3:  ldloc.s    V_2
    IL_09a5:  ldc.i4.1
    IL_09a6:  sub
    IL_09a7:  dup
    IL_09a8:  stloc.s    V_2
    IL_09aa:  ldloc.s    V_7
    IL_09ac:  stelem.i4
    IL_09ad:  ldloc.s    V_0
    IL_09af:  bne.un.s   IL_09b7

    IL_09b1:  ldloc.s    V_6
    IL_09b3:  ldc.i4.s   0
    IL_09b5:  bge.s      IL_0a0a

    IL_09b7:  ldloc.s    V_6
    IL_09b9:  ldc.i4.s   1
    IL_09bb:  bge.s      IL_0a0a

    IL_09bd:  ldloc.s    V_5
    IL_09bf:  ldloc.s    V_4
    IL_09c1:  ldc.i4.1
    IL_09c2:  sub
    IL_09c3:  dup
    IL_09c4:  stloc.s    V_4
    IL_09c6:  ldloc.s    V_0
    IL_09c8:  stelem.i4
    IL_09c9:  ldloc.s    V_5
    IL_09cb:  ldloc.s    V_4
    IL_09cd:  ldc.i4.1
    IL_09ce:  sub
    IL_09cf:  dup
    IL_09d0:  stloc.s    V_4
    IL_09d2:  ldloc.s    V_6
    IL_09d4:  ldc.i4.s   1
    IL_09d6:  add
    IL_09d7:  stelem.i4
    IL_09d8:  ldloc.s    V_3
    IL_09da:  ldloc.s    V_2
    IL_09dc:  ldc.i4.1
    IL_09dd:  sub
    IL_09de:  dup
    IL_09df:  stloc.s    V_2
    IL_09e1:  ldc.i4.s   19
    IL_09e3:  stelem.i4
    IL_09e4:  ldloc.s    V_2
    IL_09e6:  ldc.i4     0x188
    IL_09eb:  ble.s      IL_09f9

    IL_09ed:  ldloc.s    V_4
    IL_09ef:  ldc.i4     0x126
    IL_09f4:  bgt        IL_0921

    IL_09f9:  ldloc.s    V_3
    IL_09fb:  ldloc.s    V_2
    IL_09fd:  ldc.i4.1
    IL_09fe:  sub
    IL_09ff:  dup
    IL_0a00:  stloc.s    V_2
    IL_0a02:  ldc.i4.s   20
    IL_0a04:  stelem.i4
    IL_0a05:  br         IL_1cba

    IL_0a0a:  ldloc.s    V_3
    IL_0a0c:  ldloc.s    V_2
    IL_0a0e:  ldc.i4.1
    IL_0a0f:  sub
    IL_0a10:  dup
    IL_0a11:  stloc.s    V_2
    IL_0a13:  ldloc.s    V_6
    IL_0a15:  stelem.i4
    IL_0a16:  ldloc.s    V_3
    IL_0a18:  ldloc.s    V_2
    IL_0a1a:  ldc.i4.1
    IL_0a1b:  sub
    IL_0a1c:  dup
    IL_0a1d:  stloc.s    V_2
    IL_0a1f:  ldc.i4.s   11
    IL_0a21:  stelem.i4
    IL_0a22:  ldloc.s    V_5
    IL_0a24:  ldloc.s    V_4
    IL_0a26:  dup
    IL_0a27:  ldc.i4.1
    IL_0a28:  add
    IL_0a29:  stloc.s    V_4
    IL_0a2b:  ldelem.i4
    IL_0a2c:  stloc.s    V_6
    IL_0a2e:  ldarg.0
    IL_0a2f:  ldc.i4.s   6
    IL_0a31:  ldloc.s    V_6
    IL_0a33:  ldloc.s    V_0
    IL_0a35:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0a3a:  ldloc.s    V_3
    IL_0a3c:  ldloc.s    V_2
    IL_0a3e:  ldc.i4.1
    IL_0a3f:  sub
    IL_0a40:  dup
    IL_0a41:  stloc.s    V_2
    IL_0a43:  ldloc.s    V_6
    IL_0a45:  stelem.i4
    IL_0a46:  ldloc.s    V_3
    IL_0a48:  ldloc.s    V_2
    IL_0a4a:  ldc.i4.1
    IL_0a4b:  sub
    IL_0a4c:  dup
    IL_0a4d:  stloc.s    V_2
    IL_0a4f:  ldc.i4.s   8
    IL_0a51:  stelem.i4
    IL_0a52:  ldloc.s    V_5
    IL_0a54:  ldloc.s    V_4
    IL_0a56:  dup
    IL_0a57:  ldc.i4.1
    IL_0a58:  add
    IL_0a59:  stloc.s    V_4
    IL_0a5b:  ldelem.i4
    IL_0a5c:  stloc.s    V_6
    IL_0a5e:  ldloc.s    V_5
    IL_0a60:  ldloc.s    V_4
    IL_0a62:  dup
    IL_0a63:  ldc.i4.1
    IL_0a64:  add
    IL_0a65:  stloc.s    V_4
    IL_0a67:  ldelem.i4
    IL_0a68:  dup
    IL_0a69:  stloc.s    V_7
    IL_0a6b:  ldloc.s    V_3
    IL_0a6d:  ldloc.s    V_2
    IL_0a6f:  ldc.i4.1
    IL_0a70:  sub
    IL_0a71:  dup
    IL_0a72:  stloc.s    V_2
    IL_0a74:  ldloc.s    V_7
    IL_0a76:  stelem.i4
    IL_0a77:  ldloc.s    V_0
    IL_0a79:  bne.un.s   IL_0a81

    IL_0a7b:  ldloc.s    V_6
    IL_0a7d:  ldc.i4.s   0
    IL_0a7f:  bge.s      IL_0ad4

    IL_0a81:  ldloc.s    V_6
    IL_0a83:  ldc.i4.s   1
    IL_0a85:  bge.s      IL_0ad4

    IL_0a87:  ldloc.s    V_5
    IL_0a89:  ldloc.s    V_4
    IL_0a8b:  ldc.i4.1
    IL_0a8c:  sub
    IL_0a8d:  dup
    IL_0a8e:  stloc.s    V_4
    IL_0a90:  ldloc.s    V_0
    IL_0a92:  stelem.i4
    IL_0a93:  ldloc.s    V_5
    IL_0a95:  ldloc.s    V_4
    IL_0a97:  ldc.i4.1
    IL_0a98:  sub
    IL_0a99:  dup
    IL_0a9a:  stloc.s    V_4
    IL_0a9c:  ldloc.s    V_6
    IL_0a9e:  ldc.i4.s   1
    IL_0aa0:  add
    IL_0aa1:  stelem.i4
    IL_0aa2:  ldloc.s    V_3
    IL_0aa4:  ldloc.s    V_2
    IL_0aa6:  ldc.i4.1
    IL_0aa7:  sub
    IL_0aa8:  dup
    IL_0aa9:  stloc.s    V_2
    IL_0aab:  ldc.i4.s   21
    IL_0aad:  stelem.i4
    IL_0aae:  ldloc.s    V_2
    IL_0ab0:  ldc.i4     0x188
    IL_0ab5:  ble.s      IL_0ac3

    IL_0ab7:  ldloc.s    V_4
    IL_0ab9:  ldc.i4     0x126
    IL_0abe:  bgt        IL_080c

    IL_0ac3:  ldloc.s    V_3
    IL_0ac5:  ldloc.s    V_2
    IL_0ac7:  ldc.i4.1
    IL_0ac8:  sub
    IL_0ac9:  dup
    IL_0aca:  stloc.s    V_2
    IL_0acc:  ldc.i4.s   22
    IL_0ace:  stelem.i4
    IL_0acf:  br         IL_1cba

    IL_0ad4:  ldloc.s    V_3
    IL_0ad6:  ldloc.s    V_2
    IL_0ad8:  ldc.i4.1
    IL_0ad9:  sub
    IL_0ada:  dup
    IL_0adb:  stloc.s    V_2
    IL_0add:  ldloc.s    V_6
    IL_0adf:  stelem.i4
    IL_0ae0:  ldloc.s    V_3
    IL_0ae2:  ldloc.s    V_2
    IL_0ae4:  ldc.i4.1
    IL_0ae5:  sub
    IL_0ae6:  dup
    IL_0ae7:  stloc.s    V_2
    IL_0ae9:  ldc.i4.s   11
    IL_0aeb:  stelem.i4
    IL_0aec:  ldloc.s    V_5
    IL_0aee:  ldloc.s    V_4
    IL_0af0:  ldc.i4.1
    IL_0af1:  sub
    IL_0af2:  dup
    IL_0af3:  stloc.s    V_4
    IL_0af5:  ldc.i4.s   -1
    IL_0af7:  stelem.i4
    IL_0af8:  ldloc.s    V_5
    IL_0afa:  ldloc.s    V_4
    IL_0afc:  ldc.i4.1
    IL_0afd:  sub
    IL_0afe:  dup
    IL_0aff:  stloc.s    V_4
    IL_0b01:  ldc.i4.s   0
    IL_0b03:  stelem.i4
    IL_0b04:  ldloc.s    V_3
    IL_0b06:  ldloc.s    V_2
    IL_0b08:  ldc.i4.1
    IL_0b09:  sub
    IL_0b0a:  dup
    IL_0b0b:  stloc.s    V_2
    IL_0b0d:  ldc.i4.s   5
    IL_0b0f:  stelem.i4
    IL_0b10:  br         IL_0c50

    IL_0b15:  ldloc.s    V_5
    IL_0b17:  ldloc.s    V_4
    IL_0b19:  ldc.i4.1
    IL_0b1a:  sub
    IL_0b1b:  dup
    IL_0b1c:  stloc.s    V_4
    IL_0b1e:  ldloc.s    V_0
    IL_0b20:  stelem.i4
    IL_0b21:  ldloc.s    V_3
    IL_0b23:  ldloc.s    V_2
    IL_0b25:  ldc.i4.1
    IL_0b26:  sub
    IL_0b27:  dup
    IL_0b28:  stloc.s    V_2
    IL_0b2a:  ldc.i4.s   1
    IL_0b2c:  stelem.i4
    IL_0b2d:  ldloc.s    V_0
    IL_0b2f:  ldloc.s    V_10
    IL_0b31:  bge        IL_1cba

    IL_0b36:  ldloc.s    V_1
    IL_0b38:  ldloc.s    V_0
    IL_0b3a:  dup
    IL_0b3b:  ldc.i4.s   1
    IL_0b3d:  add
    IL_0b3e:  stloc.s    V_0
    IL_0b40:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0b45:  ldc.i4.s   91
    IL_0b47:  bne.un     IL_1cba

    IL_0b4c:  ldc.i4.s   1
    IL_0b4e:  ldloc.s    V_10
    IL_0b50:  ldloc.s    V_0
    IL_0b52:  sub
    IL_0b53:  bgt        IL_1cba

    IL_0b58:  ldloc.s    V_0
    IL_0b5a:  ldc.i4.s   1
    IL_0b5c:  add
    IL_0b5d:  stloc.s    V_0
    IL_0b5f:  ldc.i4.s   1
    IL_0b61:  stloc.s    V_6
    IL_0b63:  ldloc.s    V_1
    IL_0b65:  ldloc.s    V_0
    IL_0b67:  ldloc.s    V_6
    IL_0b69:  dup
    IL_0b6a:  ldc.i4.s   1
    IL_0b6c:  sub
    IL_0b6d:  stloc.s    V_6
    IL_0b6f:  sub
    IL_0b70:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0b75:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0b7a:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0b7f:  brfalse    IL_1cba

    IL_0b84:  ldloc.s    V_6
    IL_0b86:  ldc.i4.s   0
    IL_0b88:  bgt        IL_0b63

    IL_0b8d:  ldloc.s    V_10
    IL_0b8f:  ldloc.s    V_0
    IL_0b91:  sub
    IL_0b92:  dup
    IL_0b93:  stloc.s    V_7
    IL_0b95:  ldc.i4.s   1
    IL_0b97:  add
    IL_0b98:  stloc.s    V_6
    IL_0b9a:  ldloc.s    V_6
    IL_0b9c:  ldc.i4.s   1
    IL_0b9e:  sub
    IL_0b9f:  dup
    IL_0ba0:  stloc.s    V_6
    IL_0ba2:  ldc.i4.s   0
    IL_0ba4:  ble        IL_0bce

    IL_0ba9:  ldloc.s    V_1
    IL_0bab:  ldloc.s    V_0
    IL_0bad:  dup
    IL_0bae:  ldc.i4.s   1
    IL_0bb0:  add
    IL_0bb1:  stloc.s    V_0
    IL_0bb3:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0bb8:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0bbd:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0bc2:  brtrue     IL_0b9a

    IL_0bc7:  ldloc.s    V_0
    IL_0bc9:  ldc.i4.s   1
    IL_0bcb:  sub
    IL_0bcc:  stloc.s    V_0
    IL_0bce:  ldloc.s    V_7
    IL_0bd0:  ldloc.s    V_6
    IL_0bd2:  ble.s      IL_0c01

    IL_0bd4:  ldloc.s    V_3
    IL_0bd6:  ldloc.s    V_2
    IL_0bd8:  ldc.i4.1
    IL_0bd9:  sub
    IL_0bda:  dup
    IL_0bdb:  stloc.s    V_2
    IL_0bdd:  ldloc.s    V_7
    IL_0bdf:  ldloc.s    V_6
    IL_0be1:  sub
    IL_0be2:  ldc.i4.s   1
    IL_0be4:  sub
    IL_0be5:  stelem.i4
    IL_0be6:  ldloc.s    V_3
    IL_0be8:  ldloc.s    V_2
    IL_0bea:  ldc.i4.1
    IL_0beb:  sub
    IL_0bec:  dup
    IL_0bed:  stloc.s    V_2
    IL_0bef:  ldloc.s    V_0
    IL_0bf1:  ldc.i4.s   1
    IL_0bf3:  sub
    IL_0bf4:  stelem.i4
    IL_0bf5:  ldloc.s    V_3
    IL_0bf7:  ldloc.s    V_2
    IL_0bf9:  ldc.i4.1
    IL_0bfa:  sub
    IL_0bfb:  dup
    IL_0bfc:  stloc.s    V_2
    IL_0bfe:  ldc.i4.s   23
    IL_0c00:  stelem.i4
    IL_0c01:  ldloc.s    V_0
    IL_0c03:  ldloc.s    V_10
    IL_0c05:  bge        IL_1cba

    IL_0c0a:  ldloc.s    V_1
    IL_0c0c:  ldloc.s    V_0
    IL_0c0e:  dup
    IL_0c0f:  ldc.i4.s   1
    IL_0c11:  add
    IL_0c12:  stloc.s    V_0
    IL_0c14:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0c19:  ldc.i4.s   93
    IL_0c1b:  bne.un     IL_1cba

    IL_0c20:  ldloc.s    V_5
    IL_0c22:  ldloc.s    V_4
    IL_0c24:  dup
    IL_0c25:  ldc.i4.1
    IL_0c26:  add
    IL_0c27:  stloc.s    V_4
    IL_0c29:  ldelem.i4
    IL_0c2a:  stloc.s    V_6
    IL_0c2c:  ldarg.0
    IL_0c2d:  ldc.i4.s   8
    IL_0c2f:  ldloc.s    V_6
    IL_0c31:  ldloc.s    V_0
    IL_0c33:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0c38:  ldloc.s    V_3
    IL_0c3a:  ldloc.s    V_2
    IL_0c3c:  ldc.i4.1
    IL_0c3d:  sub
    IL_0c3e:  dup
    IL_0c3f:  stloc.s    V_2
    IL_0c41:  ldloc.s    V_6
    IL_0c43:  stelem.i4
    IL_0c44:  ldloc.s    V_3
    IL_0c46:  ldloc.s    V_2
    IL_0c48:  ldc.i4.1
    IL_0c49:  sub
    IL_0c4a:  dup
    IL_0c4b:  stloc.s    V_2
    IL_0c4d:  ldc.i4.s   8
    IL_0c4f:  stelem.i4
    IL_0c50:  ldloc.s    V_5
    IL_0c52:  ldloc.s    V_4
    IL_0c54:  dup
    IL_0c55:  ldc.i4.1
    IL_0c56:  add
    IL_0c57:  stloc.s    V_4
    IL_0c59:  ldelem.i4
    IL_0c5a:  stloc.s    V_6
    IL_0c5c:  ldloc.s    V_5
    IL_0c5e:  ldloc.s    V_4
    IL_0c60:  dup
    IL_0c61:  ldc.i4.1
    IL_0c62:  add
    IL_0c63:  stloc.s    V_4
    IL_0c65:  ldelem.i4
    IL_0c66:  dup
    IL_0c67:  stloc.s    V_7
    IL_0c69:  ldloc.s    V_3
    IL_0c6b:  ldloc.s    V_2
    IL_0c6d:  ldc.i4.1
    IL_0c6e:  sub
    IL_0c6f:  dup
    IL_0c70:  stloc.s    V_2
    IL_0c72:  ldloc.s    V_7
    IL_0c74:  stelem.i4
    IL_0c75:  ldloc.s    V_0
    IL_0c77:  bne.un.s   IL_0c7f

    IL_0c79:  ldloc.s    V_6
    IL_0c7b:  ldc.i4.s   0
    IL_0c7d:  bge.s      IL_0cd2

    IL_0c7f:  ldloc.s    V_6
    IL_0c81:  ldc.i4.s   1
    IL_0c83:  bge.s      IL_0cd2

    IL_0c85:  ldloc.s    V_5
    IL_0c87:  ldloc.s    V_4
    IL_0c89:  ldc.i4.1
    IL_0c8a:  sub
    IL_0c8b:  dup
    IL_0c8c:  stloc.s    V_4
    IL_0c8e:  ldloc.s    V_0
    IL_0c90:  stelem.i4
    IL_0c91:  ldloc.s    V_5
    IL_0c93:  ldloc.s    V_4
    IL_0c95:  ldc.i4.1
    IL_0c96:  sub
    IL_0c97:  dup
    IL_0c98:  stloc.s    V_4
    IL_0c9a:  ldloc.s    V_6
    IL_0c9c:  ldc.i4.s   1
    IL_0c9e:  add
    IL_0c9f:  stelem.i4
    IL_0ca0:  ldloc.s    V_3
    IL_0ca2:  ldloc.s    V_2
    IL_0ca4:  ldc.i4.1
    IL_0ca5:  sub
    IL_0ca6:  dup
    IL_0ca7:  stloc.s    V_2
    IL_0ca9:  ldc.i4.s   24
    IL_0cab:  stelem.i4
    IL_0cac:  ldloc.s    V_2
    IL_0cae:  ldc.i4     0x188
    IL_0cb3:  ble.s      IL_0cc1

    IL_0cb5:  ldloc.s    V_4
    IL_0cb7:  ldc.i4     0x126
    IL_0cbc:  bgt        IL_0b15

    IL_0cc1:  ldloc.s    V_3
    IL_0cc3:  ldloc.s    V_2
    IL_0cc5:  ldc.i4.1
    IL_0cc6:  sub
    IL_0cc7:  dup
    IL_0cc8:  stloc.s    V_2
    IL_0cca:  ldc.i4.s   25
    IL_0ccc:  stelem.i4
    IL_0ccd:  br         IL_1cba

    IL_0cd2:  ldloc.s    V_3
    IL_0cd4:  ldloc.s    V_2
    IL_0cd6:  ldc.i4.1
    IL_0cd7:  sub
    IL_0cd8:  dup
    IL_0cd9:  stloc.s    V_2
    IL_0cdb:  ldloc.s    V_6
    IL_0cdd:  stelem.i4
    IL_0cde:  ldloc.s    V_3
    IL_0ce0:  ldloc.s    V_2
    IL_0ce2:  ldc.i4.1
    IL_0ce3:  sub
    IL_0ce4:  dup
    IL_0ce5:  stloc.s    V_2
    IL_0ce7:  ldc.i4.s   11
    IL_0ce9:  stelem.i4
    IL_0cea:  ldloc.s    V_0
    IL_0cec:  ldloc.s    V_10
    IL_0cee:  bge        IL_1cba

    IL_0cf3:  ldloc.s    V_1
    IL_0cf5:  ldloc.s    V_0
    IL_0cf7:  dup
    IL_0cf8:  ldc.i4.s   1
    IL_0cfa:  add
    IL_0cfb:  stloc.s    V_0
    IL_0cfd:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0d02:  ldc.i4.s   58
    IL_0d04:  bne.un     IL_1cba

    IL_0d09:  ldloc.s    V_5
    IL_0d0b:  ldloc.s    V_4
    IL_0d0d:  ldc.i4.1
    IL_0d0e:  sub
    IL_0d0f:  dup
    IL_0d10:  stloc.s    V_4
    IL_0d12:  ldloc.s    V_0
    IL_0d14:  stelem.i4
    IL_0d15:  ldloc.s    V_3
    IL_0d17:  ldloc.s    V_2
    IL_0d19:  ldc.i4.1
    IL_0d1a:  sub
    IL_0d1b:  dup
    IL_0d1c:  stloc.s    V_2
    IL_0d1e:  ldc.i4.s   1
    IL_0d20:  stelem.i4
    IL_0d21:  ldloc.s    V_3
    IL_0d23:  ldloc.s    V_2
    IL_0d25:  ldc.i4.1
    IL_0d26:  sub
    IL_0d27:  dup
    IL_0d28:  stloc.s    V_2
    IL_0d2a:  ldloc.s    V_0
    IL_0d2c:  stelem.i4
    IL_0d2d:  ldloc.s    V_3
    IL_0d2f:  ldloc.s    V_2
    IL_0d31:  ldc.i4.1
    IL_0d32:  sub
    IL_0d33:  dup
    IL_0d34:  stloc.s    V_2
    IL_0d36:  ldc.i4.s   26
    IL_0d38:  stelem.i4
    IL_0d39:  ldc.i4.s   1
    IL_0d3b:  ldloc.s    V_10
    IL_0d3d:  ldloc.s    V_0
    IL_0d3f:  sub
    IL_0d40:  bgt        IL_1cba

    IL_0d45:  ldloc.s    V_0
    IL_0d47:  ldc.i4.s   1
    IL_0d49:  add
    IL_0d4a:  stloc.s    V_0
    IL_0d4c:  ldc.i4.s   1
    IL_0d4e:  stloc.s    V_6
    IL_0d50:  ldloc.s    V_1
    IL_0d52:  ldloc.s    V_0
    IL_0d54:  ldloc.s    V_6
    IL_0d56:  dup
    IL_0d57:  ldc.i4.s   1
    IL_0d59:  sub
    IL_0d5a:  stloc.s    V_6
    IL_0d5c:  sub
    IL_0d5d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0d62:  ldstr      bytearray (00 00 02 00 09 00 2D 00 30 00 00 00 02 00 04 00   // ......-.0.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0d67:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0d6c:  brfalse    IL_1cba

    IL_0d71:  ldloc.s    V_6
    IL_0d73:  ldc.i4.s   0
    IL_0d75:  bgt        IL_0d50

    IL_0d7a:  ldloc.s    V_10
    IL_0d7c:  ldloc.s    V_0
    IL_0d7e:  sub
    IL_0d7f:  dup
    IL_0d80:  stloc.s    V_7
    IL_0d82:  ldc.i4.s   1
    IL_0d84:  add
    IL_0d85:  stloc.s    V_6
    IL_0d87:  ldloc.s    V_6
    IL_0d89:  ldc.i4.s   1
    IL_0d8b:  sub
    IL_0d8c:  dup
    IL_0d8d:  stloc.s    V_6
    IL_0d8f:  ldc.i4.s   0
    IL_0d91:  ble        IL_0dbb

    IL_0d96:  ldloc.s    V_1
    IL_0d98:  ldloc.s    V_0
    IL_0d9a:  dup
    IL_0d9b:  ldc.i4.s   1
    IL_0d9d:  add
    IL_0d9e:  stloc.s    V_0
    IL_0da0:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0da5:  ldstr      bytearray (00 00 02 00 09 00 2D 00 30 00 00 00 02 00 04 00   // ......-.0.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0daa:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0daf:  brtrue     IL_0d87

    IL_0db4:  ldloc.s    V_0
    IL_0db6:  ldc.i4.s   1
    IL_0db8:  sub
    IL_0db9:  stloc.s    V_0
    IL_0dbb:  ldloc.s    V_7
    IL_0dbd:  ldloc.s    V_6
    IL_0dbf:  ble.s      IL_0dee

    IL_0dc1:  ldloc.s    V_3
    IL_0dc3:  ldloc.s    V_2
    IL_0dc5:  ldc.i4.1
    IL_0dc6:  sub
    IL_0dc7:  dup
    IL_0dc8:  stloc.s    V_2
    IL_0dca:  ldloc.s    V_7
    IL_0dcc:  ldloc.s    V_6
    IL_0dce:  sub
    IL_0dcf:  ldc.i4.s   1
    IL_0dd1:  sub
    IL_0dd2:  stelem.i4
    IL_0dd3:  ldloc.s    V_3
    IL_0dd5:  ldloc.s    V_2
    IL_0dd7:  ldc.i4.1
    IL_0dd8:  sub
    IL_0dd9:  dup
    IL_0dda:  stloc.s    V_2
    IL_0ddc:  ldloc.s    V_0
    IL_0dde:  ldc.i4.s   1
    IL_0de0:  sub
    IL_0de1:  stelem.i4
    IL_0de2:  ldloc.s    V_3
    IL_0de4:  ldloc.s    V_2
    IL_0de6:  ldc.i4.1
    IL_0de7:  sub
    IL_0de8:  dup
    IL_0de9:  stloc.s    V_2
    IL_0deb:  ldc.i4.s   27
    IL_0ded:  stelem.i4
    IL_0dee:  ldloc.s    V_0
    IL_0df0:  ldloc.s    V_10
    IL_0df2:  bge        IL_1cba

    IL_0df7:  ldloc.s    V_1
    IL_0df9:  ldloc.s    V_0
    IL_0dfb:  dup
    IL_0dfc:  ldc.i4.s   1
    IL_0dfe:  add
    IL_0dff:  stloc.s    V_0
    IL_0e01:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0e06:  ldc.i4.s   36
    IL_0e08:  bne.un     IL_1cba

    IL_0e0d:  br         IL_19d4

    IL_0e12:  ldloc.s    V_3
    IL_0e14:  ldloc.s    V_2
    IL_0e16:  ldc.i4.1
    IL_0e17:  sub
    IL_0e18:  dup
    IL_0e19:  stloc.s    V_2
    IL_0e1b:  ldloc.s    V_0
    IL_0e1d:  stelem.i4
    IL_0e1e:  ldloc.s    V_3
    IL_0e20:  ldloc.s    V_2
    IL_0e22:  ldc.i4.1
    IL_0e23:  sub
    IL_0e24:  dup
    IL_0e25:  stloc.s    V_2
    IL_0e27:  ldc.i4.s   28
    IL_0e29:  stelem.i4
    IL_0e2a:  ldc.i4.s   1
    IL_0e2c:  ldloc.s    V_10
    IL_0e2e:  ldloc.s    V_0
    IL_0e30:  sub
    IL_0e31:  bgt        IL_1cba

    IL_0e36:  ldloc.s    V_0
    IL_0e38:  ldc.i4.s   1
    IL_0e3a:  add
    IL_0e3b:  stloc.s    V_0
    IL_0e3d:  ldc.i4.s   1
    IL_0e3f:  stloc.s    V_6
    IL_0e41:  ldloc.s    V_1
    IL_0e43:  ldloc.s    V_0
    IL_0e45:  ldloc.s    V_6
    IL_0e47:  dup
    IL_0e48:  ldc.i4.s   1
    IL_0e4a:  sub
    IL_0e4b:  stloc.s    V_6
    IL_0e4d:  sub
    IL_0e4e:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0e53:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0e58:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0e5d:  brfalse    IL_1cba

    IL_0e62:  ldloc.s    V_6
    IL_0e64:  ldc.i4.s   0
    IL_0e66:  bgt        IL_0e41

    IL_0e6b:  ldloc.s    V_10
    IL_0e6d:  ldloc.s    V_0
    IL_0e6f:  sub
    IL_0e70:  dup
    IL_0e71:  stloc.s    V_7
    IL_0e73:  ldc.i4.s   1
    IL_0e75:  add
    IL_0e76:  stloc.s    V_6
    IL_0e78:  ldloc.s    V_6
    IL_0e7a:  ldc.i4.s   1
    IL_0e7c:  sub
    IL_0e7d:  dup
    IL_0e7e:  stloc.s    V_6
    IL_0e80:  ldc.i4.s   0
    IL_0e82:  ble        IL_0eac

    IL_0e87:  ldloc.s    V_1
    IL_0e89:  ldloc.s    V_0
    IL_0e8b:  dup
    IL_0e8c:  ldc.i4.s   1
    IL_0e8e:  add
    IL_0e8f:  stloc.s    V_0
    IL_0e91:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0e96:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0e9b:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0ea0:  brtrue     IL_0e78

    IL_0ea5:  ldloc.s    V_0
    IL_0ea7:  ldc.i4.s   1
    IL_0ea9:  sub
    IL_0eaa:  stloc.s    V_0
    IL_0eac:  ldloc.s    V_7
    IL_0eae:  ldloc.s    V_6
    IL_0eb0:  ble.s      IL_0edf

    IL_0eb2:  ldloc.s    V_3
    IL_0eb4:  ldloc.s    V_2
    IL_0eb6:  ldc.i4.1
    IL_0eb7:  sub
    IL_0eb8:  dup
    IL_0eb9:  stloc.s    V_2
    IL_0ebb:  ldloc.s    V_7
    IL_0ebd:  ldloc.s    V_6
    IL_0ebf:  sub
    IL_0ec0:  ldc.i4.s   1
    IL_0ec2:  sub
    IL_0ec3:  stelem.i4
    IL_0ec4:  ldloc.s    V_3
    IL_0ec6:  ldloc.s    V_2
    IL_0ec8:  ldc.i4.1
    IL_0ec9:  sub
    IL_0eca:  dup
    IL_0ecb:  stloc.s    V_2
    IL_0ecd:  ldloc.s    V_0
    IL_0ecf:  ldc.i4.s   1
    IL_0ed1:  sub
    IL_0ed2:  stelem.i4
    IL_0ed3:  ldloc.s    V_3
    IL_0ed5:  ldloc.s    V_2
    IL_0ed7:  ldc.i4.1
    IL_0ed8:  sub
    IL_0ed9:  dup
    IL_0eda:  stloc.s    V_2
    IL_0edc:  ldc.i4.s   29
    IL_0ede:  stelem.i4
    IL_0edf:  ldc.i4.s   2
    IL_0ee1:  ldloc.s    V_10
    IL_0ee3:  ldloc.s    V_0
    IL_0ee5:  sub
    IL_0ee6:  bgt        IL_1cba

    IL_0eeb:  ldloc.s    V_1
    IL_0eed:  ldloc.s    V_0
    IL_0eef:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0ef4:  ldc.i4.s   58
    IL_0ef6:  bne.un     IL_1cba

    IL_0efb:  ldloc.s    V_1
    IL_0efd:  ldloc.s    V_0
    IL_0eff:  ldc.i4.s   1
    IL_0f01:  add
    IL_0f02:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0f07:  ldc.i4.s   123
    IL_0f09:  bne.un     IL_1cba

    IL_0f0e:  ldloc.s    V_0
    IL_0f10:  ldc.i4.s   2
    IL_0f12:  add
    IL_0f13:  stloc.s    V_0
    IL_0f15:  br         IL_19d4

    IL_0f1a:  ldc.i4.s   1
    IL_0f1c:  ldloc.s    V_10
    IL_0f1e:  ldloc.s    V_0
    IL_0f20:  sub
    IL_0f21:  bgt        IL_1cba

    IL_0f26:  ldloc.s    V_0
    IL_0f28:  ldc.i4.s   1
    IL_0f2a:  add
    IL_0f2b:  stloc.s    V_0
    IL_0f2d:  ldc.i4.s   1
    IL_0f2f:  stloc.s    V_6
    IL_0f31:  ldloc.s    V_1
    IL_0f33:  ldloc.s    V_0
    IL_0f35:  ldloc.s    V_6
    IL_0f37:  dup
    IL_0f38:  ldc.i4.s   1
    IL_0f3a:  sub
    IL_0f3b:  stloc.s    V_6
    IL_0f3d:  sub
    IL_0f3e:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0f43:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0f48:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0f4d:  brfalse    IL_1cba

    IL_0f52:  ldloc.s    V_6
    IL_0f54:  ldc.i4.s   0
    IL_0f56:  bgt        IL_0f31

    IL_0f5b:  ldloc.s    V_10
    IL_0f5d:  ldloc.s    V_0
    IL_0f5f:  sub
    IL_0f60:  dup
    IL_0f61:  stloc.s    V_7
    IL_0f63:  ldc.i4.s   1
    IL_0f65:  add
    IL_0f66:  stloc.s    V_6
    IL_0f68:  ldloc.s    V_6
    IL_0f6a:  ldc.i4.s   1
    IL_0f6c:  sub
    IL_0f6d:  dup
    IL_0f6e:  stloc.s    V_6
    IL_0f70:  ldc.i4.s   0
    IL_0f72:  ble        IL_0f9c

    IL_0f77:  ldloc.s    V_1
    IL_0f79:  ldloc.s    V_0
    IL_0f7b:  dup
    IL_0f7c:  ldc.i4.s   1
    IL_0f7e:  add
    IL_0f7f:  stloc.s    V_0
    IL_0f81:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0f86:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0f8b:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0f90:  brtrue     IL_0f68

    IL_0f95:  ldloc.s    V_0
    IL_0f97:  ldc.i4.s   1
    IL_0f99:  sub
    IL_0f9a:  stloc.s    V_0
    IL_0f9c:  ldloc.s    V_7
    IL_0f9e:  ldloc.s    V_6
    IL_0fa0:  ble.s      IL_0fcf

    IL_0fa2:  ldloc.s    V_3
    IL_0fa4:  ldloc.s    V_2
    IL_0fa6:  ldc.i4.1
    IL_0fa7:  sub
    IL_0fa8:  dup
    IL_0fa9:  stloc.s    V_2
    IL_0fab:  ldloc.s    V_7
    IL_0fad:  ldloc.s    V_6
    IL_0faf:  sub
    IL_0fb0:  ldc.i4.s   1
    IL_0fb2:  sub
    IL_0fb3:  stelem.i4
    IL_0fb4:  ldloc.s    V_3
    IL_0fb6:  ldloc.s    V_2
    IL_0fb8:  ldc.i4.1
    IL_0fb9:  sub
    IL_0fba:  dup
    IL_0fbb:  stloc.s    V_2
    IL_0fbd:  ldloc.s    V_0
    IL_0fbf:  ldc.i4.s   1
    IL_0fc1:  sub
    IL_0fc2:  stelem.i4
    IL_0fc3:  ldloc.s    V_3
    IL_0fc5:  ldloc.s    V_2
    IL_0fc7:  ldc.i4.1
    IL_0fc8:  sub
    IL_0fc9:  dup
    IL_0fca:  stloc.s    V_2
    IL_0fcc:  ldc.i4.s   30
    IL_0fce:  stelem.i4
    IL_0fcf:  ldloc.s    V_0
    IL_0fd1:  ldloc.s    V_10
    IL_0fd3:  bge        IL_1cba

    IL_0fd8:  ldloc.s    V_1
    IL_0fda:  ldloc.s    V_0
    IL_0fdc:  dup
    IL_0fdd:  ldc.i4.s   1
    IL_0fdf:  add
    IL_0fe0:  stloc.s    V_0
    IL_0fe2:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0fe7:  ldc.i4.s   63
    IL_0fe9:  bne.un     IL_1cba

    IL_0fee:  ldc.i4.s   1
    IL_0ff0:  ldloc.s    V_10
    IL_0ff2:  ldloc.s    V_0
    IL_0ff4:  sub
    IL_0ff5:  bgt        IL_1cba

    IL_0ffa:  ldloc.s    V_0
    IL_0ffc:  ldc.i4.s   1
    IL_0ffe:  add
    IL_0fff:  stloc.s    V_0
    IL_1001:  ldc.i4.s   1
    IL_1003:  stloc.s    V_6
    IL_1005:  ldloc.s    V_1
    IL_1007:  ldloc.s    V_0
    IL_1009:  ldloc.s    V_6
    IL_100b:  dup
    IL_100c:  ldc.i4.s   1
    IL_100e:  sub
    IL_100f:  stloc.s    V_6
    IL_1011:  sub
    IL_1012:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1017:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_101c:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1021:  brfalse    IL_1cba

    IL_1026:  ldloc.s    V_6
    IL_1028:  ldc.i4.s   0
    IL_102a:  bgt        IL_1005

    IL_102f:  ldloc.s    V_10
    IL_1031:  ldloc.s    V_0
    IL_1033:  sub
    IL_1034:  dup
    IL_1035:  stloc.s    V_7
    IL_1037:  ldc.i4.s   1
    IL_1039:  add
    IL_103a:  stloc.s    V_6
    IL_103c:  ldloc.s    V_6
    IL_103e:  ldc.i4.s   1
    IL_1040:  sub
    IL_1041:  dup
    IL_1042:  stloc.s    V_6
    IL_1044:  ldc.i4.s   0
    IL_1046:  ble        IL_1070

    IL_104b:  ldloc.s    V_1
    IL_104d:  ldloc.s    V_0
    IL_104f:  dup
    IL_1050:  ldc.i4.s   1
    IL_1052:  add
    IL_1053:  stloc.s    V_0
    IL_1055:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_105a:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_105f:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1064:  brtrue     IL_103c

    IL_1069:  ldloc.s    V_0
    IL_106b:  ldc.i4.s   1
    IL_106d:  sub
    IL_106e:  stloc.s    V_0
    IL_1070:  ldloc.s    V_7
    IL_1072:  ldloc.s    V_6
    IL_1074:  ble.s      IL_10a3

    IL_1076:  ldloc.s    V_3
    IL_1078:  ldloc.s    V_2
    IL_107a:  ldc.i4.1
    IL_107b:  sub
    IL_107c:  dup
    IL_107d:  stloc.s    V_2
    IL_107f:  ldloc.s    V_7
    IL_1081:  ldloc.s    V_6
    IL_1083:  sub
    IL_1084:  ldc.i4.s   1
    IL_1086:  sub
    IL_1087:  stelem.i4
    IL_1088:  ldloc.s    V_3
    IL_108a:  ldloc.s    V_2
    IL_108c:  ldc.i4.1
    IL_108d:  sub
    IL_108e:  dup
    IL_108f:  stloc.s    V_2
    IL_1091:  ldloc.s    V_0
    IL_1093:  ldc.i4.s   1
    IL_1095:  sub
    IL_1096:  stelem.i4
    IL_1097:  ldloc.s    V_3
    IL_1099:  ldloc.s    V_2
    IL_109b:  ldc.i4.1
    IL_109c:  sub
    IL_109d:  dup
    IL_109e:  stloc.s    V_2
    IL_10a0:  ldc.i4.s   31
    IL_10a2:  stelem.i4
    IL_10a3:  ldloc.s    V_5
    IL_10a5:  ldloc.s    V_4
    IL_10a7:  ldc.i4.1
    IL_10a8:  sub
    IL_10a9:  dup
    IL_10aa:  stloc.s    V_4
    IL_10ac:  ldc.i4.s   -1
    IL_10ae:  stelem.i4
    IL_10af:  ldloc.s    V_5
    IL_10b1:  ldloc.s    V_4
    IL_10b3:  ldc.i4.1
    IL_10b4:  sub
    IL_10b5:  dup
    IL_10b6:  stloc.s    V_4
    IL_10b8:  ldc.i4.s   0
    IL_10ba:  stelem.i4
    IL_10bb:  ldloc.s    V_3
    IL_10bd:  ldloc.s    V_2
    IL_10bf:  ldc.i4.1
    IL_10c0:  sub
    IL_10c1:  dup
    IL_10c2:  stloc.s    V_2
    IL_10c4:  ldc.i4.s   5
    IL_10c6:  stelem.i4
    IL_10c7:  br         IL_191b

    IL_10cc:  ldloc.s    V_5
    IL_10ce:  ldloc.s    V_4
    IL_10d0:  ldc.i4.1
    IL_10d1:  sub
    IL_10d2:  dup
    IL_10d3:  stloc.s    V_4
    IL_10d5:  ldloc.s    V_0
    IL_10d7:  stelem.i4
    IL_10d8:  ldloc.s    V_3
    IL_10da:  ldloc.s    V_2
    IL_10dc:  ldc.i4.1
    IL_10dd:  sub
    IL_10de:  dup
    IL_10df:  stloc.s    V_2
    IL_10e1:  ldc.i4.s   1
    IL_10e3:  stelem.i4
    IL_10e4:  ldloc.s    V_0
    IL_10e6:  ldloc.s    V_10
    IL_10e8:  bge        IL_1cba

    IL_10ed:  ldloc.s    V_1
    IL_10ef:  ldloc.s    V_0
    IL_10f1:  dup
    IL_10f2:  ldc.i4.s   1
    IL_10f4:  add
    IL_10f5:  stloc.s    V_0
    IL_10f7:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_10fc:  ldc.i4.s   44
    IL_10fe:  bne.un     IL_1cba

    IL_1103:  ldloc.s    V_5
    IL_1105:  ldloc.s    V_4
    IL_1107:  ldc.i4.1
    IL_1108:  sub
    IL_1109:  dup
    IL_110a:  stloc.s    V_4
    IL_110c:  ldc.i4.s   -1
    IL_110e:  stelem.i4
    IL_110f:  ldloc.s    V_3
    IL_1111:  ldloc.s    V_2
    IL_1113:  ldc.i4.1
    IL_1114:  sub
    IL_1115:  dup
    IL_1116:  stloc.s    V_2
    IL_1118:  ldc.i4.s   1
    IL_111a:  stelem.i4
    IL_111b:  br         IL_1878

    IL_1120:  ldloc.s    V_5
    IL_1122:  ldloc.s    V_4
    IL_1124:  ldc.i4.1
    IL_1125:  sub
    IL_1126:  dup
    IL_1127:  stloc.s    V_4
    IL_1129:  ldloc.s    V_0
    IL_112b:  stelem.i4
    IL_112c:  ldloc.s    V_3
    IL_112e:  ldloc.s    V_2
    IL_1130:  ldc.i4.1
    IL_1131:  sub
    IL_1132:  dup
    IL_1133:  stloc.s    V_2
    IL_1135:  ldc.i4.s   1
    IL_1137:  stelem.i4
    IL_1138:  ldloc.s    V_5
    IL_113a:  ldloc.s    V_4
    IL_113c:  ldc.i4.1
    IL_113d:  sub
    IL_113e:  dup
    IL_113f:  stloc.s    V_4
    IL_1141:  ldc.i4.s   -1
    IL_1143:  stelem.i4
    IL_1144:  ldloc.s    V_5
    IL_1146:  ldloc.s    V_4
    IL_1148:  ldc.i4.1
    IL_1149:  sub
    IL_114a:  dup
    IL_114b:  stloc.s    V_4
    IL_114d:  ldc.i4.s   0
    IL_114f:  stelem.i4
    IL_1150:  ldloc.s    V_3
    IL_1152:  ldloc.s    V_2
    IL_1154:  ldc.i4.1
    IL_1155:  sub
    IL_1156:  dup
    IL_1157:  stloc.s    V_2
    IL_1159:  ldc.i4.s   5
    IL_115b:  stelem.i4
    IL_115c:  br         IL_11c8

    IL_1161:  ldloc.s    V_5
    IL_1163:  ldloc.s    V_4
    IL_1165:  ldc.i4.1
    IL_1166:  sub
    IL_1167:  dup
    IL_1168:  stloc.s    V_4
    IL_116a:  ldloc.s    V_0
    IL_116c:  stelem.i4
    IL_116d:  ldloc.s    V_3
    IL_116f:  ldloc.s    V_2
    IL_1171:  ldc.i4.1
    IL_1172:  sub
    IL_1173:  dup
    IL_1174:  stloc.s    V_2
    IL_1176:  ldc.i4.s   1
    IL_1178:  stelem.i4
    IL_1179:  ldloc.s    V_0
    IL_117b:  ldloc.s    V_10
    IL_117d:  bge        IL_1cba

    IL_1182:  ldloc.s    V_1
    IL_1184:  ldloc.s    V_0
    IL_1186:  dup
    IL_1187:  ldc.i4.s   1
    IL_1189:  add
    IL_118a:  stloc.s    V_0
    IL_118c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1191:  ldc.i4.s   124
    IL_1193:  bne.un     IL_1cba

    IL_1198:  ldloc.s    V_5
    IL_119a:  ldloc.s    V_4
    IL_119c:  dup
    IL_119d:  ldc.i4.1
    IL_119e:  add
    IL_119f:  stloc.s    V_4
    IL_11a1:  ldelem.i4
    IL_11a2:  stloc.s    V_6
    IL_11a4:  ldarg.0
    IL_11a5:  ldc.i4.s   12
    IL_11a7:  ldloc.s    V_6
    IL_11a9:  ldloc.s    V_0
    IL_11ab:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_11b0:  ldloc.s    V_3
    IL_11b2:  ldloc.s    V_2
    IL_11b4:  ldc.i4.1
    IL_11b5:  sub
    IL_11b6:  dup
    IL_11b7:  stloc.s    V_2
    IL_11b9:  ldloc.s    V_6
    IL_11bb:  stelem.i4
    IL_11bc:  ldloc.s    V_3
    IL_11be:  ldloc.s    V_2
    IL_11c0:  ldc.i4.1
    IL_11c1:  sub
    IL_11c2:  dup
    IL_11c3:  stloc.s    V_2
    IL_11c5:  ldc.i4.s   8
    IL_11c7:  stelem.i4
    IL_11c8:  ldloc.s    V_5
    IL_11ca:  ldloc.s    V_4
    IL_11cc:  dup
    IL_11cd:  ldc.i4.1
    IL_11ce:  add
    IL_11cf:  stloc.s    V_4
    IL_11d1:  ldelem.i4
    IL_11d2:  stloc.s    V_6
    IL_11d4:  ldloc.s    V_5
    IL_11d6:  ldloc.s    V_4
    IL_11d8:  dup
    IL_11d9:  ldc.i4.1
    IL_11da:  add
    IL_11db:  stloc.s    V_4
    IL_11dd:  ldelem.i4
    IL_11de:  dup
    IL_11df:  stloc.s    V_7
    IL_11e1:  ldloc.s    V_3
    IL_11e3:  ldloc.s    V_2
    IL_11e5:  ldc.i4.1
    IL_11e6:  sub
    IL_11e7:  dup
    IL_11e8:  stloc.s    V_2
    IL_11ea:  ldloc.s    V_7
    IL_11ec:  stelem.i4
    IL_11ed:  ldloc.s    V_0
    IL_11ef:  bne.un.s   IL_11f7

    IL_11f1:  ldloc.s    V_6
    IL_11f3:  ldc.i4.s   0
    IL_11f5:  bge.s      IL_124a

    IL_11f7:  ldloc.s    V_6
    IL_11f9:  ldc.i4.s   1
    IL_11fb:  bge.s      IL_124a

    IL_11fd:  ldloc.s    V_5
    IL_11ff:  ldloc.s    V_4
    IL_1201:  ldc.i4.1
    IL_1202:  sub
    IL_1203:  dup
    IL_1204:  stloc.s    V_4
    IL_1206:  ldloc.s    V_0
    IL_1208:  stelem.i4
    IL_1209:  ldloc.s    V_5
    IL_120b:  ldloc.s    V_4
    IL_120d:  ldc.i4.1
    IL_120e:  sub
    IL_120f:  dup
    IL_1210:  stloc.s    V_4
    IL_1212:  ldloc.s    V_6
    IL_1214:  ldc.i4.s   1
    IL_1216:  add
    IL_1217:  stelem.i4
    IL_1218:  ldloc.s    V_3
    IL_121a:  ldloc.s    V_2
    IL_121c:  ldc.i4.1
    IL_121d:  sub
    IL_121e:  dup
    IL_121f:  stloc.s    V_2
    IL_1221:  ldc.i4.s   32
    IL_1223:  stelem.i4
    IL_1224:  ldloc.s    V_2
    IL_1226:  ldc.i4     0x188
    IL_122b:  ble.s      IL_1239

    IL_122d:  ldloc.s    V_4
    IL_122f:  ldc.i4     0x126
    IL_1234:  bgt        IL_1161

    IL_1239:  ldloc.s    V_3
    IL_123b:  ldloc.s    V_2
    IL_123d:  ldc.i4.1
    IL_123e:  sub
    IL_123f:  dup
    IL_1240:  stloc.s    V_2
    IL_1242:  ldc.i4.s   33
    IL_1244:  stelem.i4
    IL_1245:  br         IL_1cba

    IL_124a:  ldloc.s    V_3
    IL_124c:  ldloc.s    V_2
    IL_124e:  ldc.i4.1
    IL_124f:  sub
    IL_1250:  dup
    IL_1251:  stloc.s    V_2
    IL_1253:  ldloc.s    V_6
    IL_1255:  stelem.i4
    IL_1256:  ldloc.s    V_3
    IL_1258:  ldloc.s    V_2
    IL_125a:  ldc.i4.1
    IL_125b:  sub
    IL_125c:  dup
    IL_125d:  stloc.s    V_2
    IL_125f:  ldc.i4.s   11
    IL_1261:  stelem.i4
    IL_1262:  ldloc.s    V_5
    IL_1264:  ldloc.s    V_4
    IL_1266:  ldc.i4.1
    IL_1267:  sub
    IL_1268:  dup
    IL_1269:  stloc.s    V_4
    IL_126b:  ldc.i4.s   -1
    IL_126d:  stelem.i4
    IL_126e:  ldloc.s    V_5
    IL_1270:  ldloc.s    V_4
    IL_1272:  ldc.i4.1
    IL_1273:  sub
    IL_1274:  dup
    IL_1275:  stloc.s    V_4
    IL_1277:  ldc.i4.s   0
    IL_1279:  stelem.i4
    IL_127a:  ldloc.s    V_3
    IL_127c:  ldloc.s    V_2
    IL_127e:  ldc.i4.1
    IL_127f:  sub
    IL_1280:  dup
    IL_1281:  stloc.s    V_2
    IL_1283:  ldc.i4.s   5
    IL_1285:  stelem.i4
    IL_1286:  br         IL_17ae

    IL_128b:  ldloc.s    V_5
    IL_128d:  ldloc.s    V_4
    IL_128f:  ldc.i4.1
    IL_1290:  sub
    IL_1291:  dup
    IL_1292:  stloc.s    V_4
    IL_1294:  ldloc.s    V_0
    IL_1296:  stelem.i4
    IL_1297:  ldloc.s    V_3
    IL_1299:  ldloc.s    V_2
    IL_129b:  ldc.i4.1
    IL_129c:  sub
    IL_129d:  dup
    IL_129e:  stloc.s    V_2
    IL_12a0:  ldc.i4.s   1
    IL_12a2:  stelem.i4
    IL_12a3:  ldloc.s    V_3
    IL_12a5:  ldloc.s    V_2
    IL_12a7:  ldc.i4.1
    IL_12a8:  sub
    IL_12a9:  dup
    IL_12aa:  stloc.s    V_2
    IL_12ac:  ldloc.s    V_0
    IL_12ae:  stelem.i4
    IL_12af:  ldloc.s    V_3
    IL_12b1:  ldloc.s    V_2
    IL_12b3:  ldc.i4.1
    IL_12b4:  sub
    IL_12b5:  dup
    IL_12b6:  stloc.s    V_2
    IL_12b8:  ldc.i4.s   34
    IL_12ba:  stelem.i4
    IL_12bb:  ldloc.s    V_10
    IL_12bd:  ldloc.s    V_0
    IL_12bf:  sub
    IL_12c0:  dup
    IL_12c1:  stloc.s    V_7
    IL_12c3:  ldc.i4.s   1
    IL_12c5:  add
    IL_12c6:  stloc.s    V_6
    IL_12c8:  ldloc.s    V_6
    IL_12ca:  ldc.i4.s   1
    IL_12cc:  sub
    IL_12cd:  dup
    IL_12ce:  stloc.s    V_6
    IL_12d0:  ldc.i4.s   0
    IL_12d2:  ble        IL_12fc

    IL_12d7:  ldloc.s    V_1
    IL_12d9:  ldloc.s    V_0
    IL_12db:  dup
    IL_12dc:  ldc.i4.s   1
    IL_12de:  add
    IL_12df:  stloc.s    V_0
    IL_12e1:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_12e6:  ldstr      bytearray (00 00 0A 00 00 00 23 00 24 00 2A 00 2C 00 2D 00   // ......#.$.*.,.-.
                                    2F 00 5E 00 5F 00 7E 00 7F 00 )                   // /.^._.~...
    IL_12eb:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_12f0:  brtrue     IL_12c8

    IL_12f5:  ldloc.s    V_0
    IL_12f7:  ldc.i4.s   1
    IL_12f9:  sub
    IL_12fa:  stloc.s    V_0
    IL_12fc:  ldloc.s    V_7
    IL_12fe:  ldloc.s    V_6
    IL_1300:  ble.s      IL_132f

    IL_1302:  ldloc.s    V_3
    IL_1304:  ldloc.s    V_2
    IL_1306:  ldc.i4.1
    IL_1307:  sub
    IL_1308:  dup
    IL_1309:  stloc.s    V_2
    IL_130b:  ldloc.s    V_7
    IL_130d:  ldloc.s    V_6
    IL_130f:  sub
    IL_1310:  ldc.i4.s   1
    IL_1312:  sub
    IL_1313:  stelem.i4
    IL_1314:  ldloc.s    V_3
    IL_1316:  ldloc.s    V_2
    IL_1318:  ldc.i4.1
    IL_1319:  sub
    IL_131a:  dup
    IL_131b:  stloc.s    V_2
    IL_131d:  ldloc.s    V_0
    IL_131f:  ldc.i4.s   1
    IL_1321:  sub
    IL_1322:  stelem.i4
    IL_1323:  ldloc.s    V_3
    IL_1325:  ldloc.s    V_2
    IL_1327:  ldc.i4.1
    IL_1328:  sub
    IL_1329:  dup
    IL_132a:  stloc.s    V_2
    IL_132c:  ldc.i4.s   35
    IL_132e:  stelem.i4
    IL_132f:  ldloc.s    V_5
    IL_1331:  ldloc.s    V_4
    IL_1333:  ldc.i4.1
    IL_1334:  sub
    IL_1335:  dup
    IL_1336:  stloc.s    V_4
    IL_1338:  ldloc.s    V_0
    IL_133a:  stelem.i4
    IL_133b:  ldloc.s    V_3
    IL_133d:  ldloc.s    V_2
    IL_133f:  ldc.i4.1
    IL_1340:  sub
    IL_1341:  dup
    IL_1342:  stloc.s    V_2
    IL_1344:  ldc.i4.s   1
    IL_1346:  stelem.i4
    IL_1347:  ldloc.s    V_0
    IL_1349:  ldloc.s    V_10
    IL_134b:  bge        IL_1cba

    IL_1350:  ldloc.s    V_1
    IL_1352:  ldloc.s    V_0
    IL_1354:  dup
    IL_1355:  ldc.i4.s   1
    IL_1357:  add
    IL_1358:  stloc.s    V_0
    IL_135a:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_135f:  ldstr      bytearray (00 00 02 00 09 00 2B 00 2C 00 00 00 02 00 04 00   // ......+.,.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_1364:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1369:  brfalse    IL_1cba

    IL_136e:  ldloc.s    V_10
    IL_1370:  ldloc.s    V_0
    IL_1372:  sub
    IL_1373:  dup
    IL_1374:  stloc.s    V_7
    IL_1376:  ldc.i4.s   1
    IL_1378:  add
    IL_1379:  stloc.s    V_6
    IL_137b:  ldloc.s    V_6
    IL_137d:  ldc.i4.s   1
    IL_137f:  sub
    IL_1380:  dup
    IL_1381:  stloc.s    V_6
    IL_1383:  ldc.i4.s   0
    IL_1385:  ble.s      IL_13a1

    IL_1387:  ldloc.s    V_1
    IL_1389:  ldloc.s    V_0
    IL_138b:  dup
    IL_138c:  ldc.i4.s   1
    IL_138e:  add
    IL_138f:  stloc.s    V_0
    IL_1391:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1396:  ldc.i4.s   36
    IL_1398:  bne.un.s   IL_137b

    IL_139a:  ldloc.s    V_0
    IL_139c:  ldc.i4.s   1
    IL_139e:  sub
    IL_139f:  stloc.s    V_0
    IL_13a1:  ldloc.s    V_7
    IL_13a3:  ldloc.s    V_6
    IL_13a5:  ble.s      IL_13d4

    IL_13a7:  ldloc.s    V_3
    IL_13a9:  ldloc.s    V_2
    IL_13ab:  ldc.i4.1
    IL_13ac:  sub
    IL_13ad:  dup
    IL_13ae:  stloc.s    V_2
    IL_13b0:  ldloc.s    V_7
    IL_13b2:  ldloc.s    V_6
    IL_13b4:  sub
    IL_13b5:  ldc.i4.s   1
    IL_13b7:  sub
    IL_13b8:  stelem.i4
    IL_13b9:  ldloc.s    V_3
    IL_13bb:  ldloc.s    V_2
    IL_13bd:  ldc.i4.1
    IL_13be:  sub
    IL_13bf:  dup
    IL_13c0:  stloc.s    V_2
    IL_13c2:  ldloc.s    V_0
    IL_13c4:  ldc.i4.s   1
    IL_13c6:  sub
    IL_13c7:  stelem.i4
    IL_13c8:  ldloc.s    V_3
    IL_13ca:  ldloc.s    V_2
    IL_13cc:  ldc.i4.1
    IL_13cd:  sub
    IL_13ce:  dup
    IL_13cf:  stloc.s    V_2
    IL_13d1:  ldc.i4.s   36
    IL_13d3:  stelem.i4
    IL_13d4:  ldloc.s    V_5
    IL_13d6:  ldloc.s    V_4
    IL_13d8:  dup
    IL_13d9:  ldc.i4.1
    IL_13da:  add
    IL_13db:  stloc.s    V_4
    IL_13dd:  ldelem.i4
    IL_13de:  stloc.s    V_6
    IL_13e0:  ldarg.0
    IL_13e1:  ldc.i4.s   14
    IL_13e3:  ldloc.s    V_6
    IL_13e5:  ldloc.s    V_0
    IL_13e7:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_13ec:  ldloc.s    V_3
    IL_13ee:  ldloc.s    V_2
    IL_13f0:  ldc.i4.1
    IL_13f1:  sub
    IL_13f2:  dup
    IL_13f3:  stloc.s    V_2
    IL_13f5:  ldloc.s    V_6
    IL_13f7:  stelem.i4
    IL_13f8:  ldloc.s    V_3
    IL_13fa:  ldloc.s    V_2
    IL_13fc:  ldc.i4.1
    IL_13fd:  sub
    IL_13fe:  dup
    IL_13ff:  stloc.s    V_2
    IL_1401:  ldc.i4.s   8
    IL_1403:  stelem.i4
    IL_1404:  br         IL_177e

    IL_1409:  ldloc.s    V_3
    IL_140b:  ldloc.s    V_2
    IL_140d:  ldc.i4.1
    IL_140e:  sub
    IL_140f:  dup
    IL_1410:  stloc.s    V_2
    IL_1412:  ldloc.s    V_0
    IL_1414:  stelem.i4
    IL_1415:  ldloc.s    V_3
    IL_1417:  ldloc.s    V_2
    IL_1419:  ldc.i4.1
    IL_141a:  sub
    IL_141b:  dup
    IL_141c:  stloc.s    V_2
    IL_141e:  ldc.i4.s   37
    IL_1420:  stelem.i4
    IL_1421:  ldloc.s    V_0
    IL_1423:  ldloc.s    V_10
    IL_1425:  bge        IL_1cba

    IL_142a:  ldloc.s    V_1
    IL_142c:  ldloc.s    V_0
    IL_142e:  dup
    IL_142f:  ldc.i4.s   1
    IL_1431:  add
    IL_1432:  stloc.s    V_0
    IL_1434:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1439:  ldc.i4.s   61
    IL_143b:  bne.un     IL_1cba

    IL_1440:  ldloc.s    V_5
    IL_1442:  ldloc.s    V_4
    IL_1444:  ldc.i4.1
    IL_1445:  sub
    IL_1446:  dup
    IL_1447:  stloc.s    V_4
    IL_1449:  ldloc.s    V_0
    IL_144b:  stelem.i4
    IL_144c:  ldloc.s    V_3
    IL_144e:  ldloc.s    V_2
    IL_1450:  ldc.i4.1
    IL_1451:  sub
    IL_1452:  dup
    IL_1453:  stloc.s    V_2
    IL_1455:  ldc.i4.s   1
    IL_1457:  stelem.i4
    IL_1458:  ldloc.s    V_0
    IL_145a:  ldloc.s    V_10
    IL_145c:  bge        IL_1cba

    IL_1461:  ldloc.s    V_1
    IL_1463:  ldloc.s    V_0
    IL_1465:  dup
    IL_1466:  ldc.i4.s   1
    IL_1468:  add
    IL_1469:  stloc.s    V_0
    IL_146b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1470:  ldstr      bytearray (00 00 02 00 01 00 2B 00 2C 00 9C FF )             // ......+.,...
    IL_1475:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_147a:  brfalse    IL_1cba

    IL_147f:  ldloc.s    V_10
    IL_1481:  ldloc.s    V_0
    IL_1483:  sub
    IL_1484:  dup
    IL_1485:  stloc.s    V_7
    IL_1487:  ldc.i4.s   1
    IL_1489:  add
    IL_148a:  stloc.s    V_6
    IL_148c:  ldloc.s    V_6
    IL_148e:  ldc.i4.s   1
    IL_1490:  sub
    IL_1491:  dup
    IL_1492:  stloc.s    V_6
    IL_1494:  ldc.i4.s   0
    IL_1496:  ble.s      IL_14b2

    IL_1498:  ldloc.s    V_1
    IL_149a:  ldloc.s    V_0
    IL_149c:  dup
    IL_149d:  ldc.i4.s   1
    IL_149f:  add
    IL_14a0:  stloc.s    V_0
    IL_14a2:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_14a7:  ldc.i4.s   36
    IL_14a9:  bne.un.s   IL_148c

    IL_14ab:  ldloc.s    V_0
    IL_14ad:  ldc.i4.s   1
    IL_14af:  sub
    IL_14b0:  stloc.s    V_0
    IL_14b2:  ldloc.s    V_7
    IL_14b4:  ldloc.s    V_6
    IL_14b6:  ble.s      IL_14e5

    IL_14b8:  ldloc.s    V_3
    IL_14ba:  ldloc.s    V_2
    IL_14bc:  ldc.i4.1
    IL_14bd:  sub
    IL_14be:  dup
    IL_14bf:  stloc.s    V_2
    IL_14c1:  ldloc.s    V_7
    IL_14c3:  ldloc.s    V_6
    IL_14c5:  sub
    IL_14c6:  ldc.i4.s   1
    IL_14c8:  sub
    IL_14c9:  stelem.i4
    IL_14ca:  ldloc.s    V_3
    IL_14cc:  ldloc.s    V_2
    IL_14ce:  ldc.i4.1
    IL_14cf:  sub
    IL_14d0:  dup
    IL_14d1:  stloc.s    V_2
    IL_14d3:  ldloc.s    V_0
    IL_14d5:  ldc.i4.s   1
    IL_14d7:  sub
    IL_14d8:  stelem.i4
    IL_14d9:  ldloc.s    V_3
    IL_14db:  ldloc.s    V_2
    IL_14dd:  ldc.i4.1
    IL_14de:  sub
    IL_14df:  dup
    IL_14e0:  stloc.s    V_2
    IL_14e2:  ldc.i4.s   38
    IL_14e4:  stelem.i4
    IL_14e5:  ldloc.s    V_5
    IL_14e7:  ldloc.s    V_4
    IL_14e9:  dup
    IL_14ea:  ldc.i4.1
    IL_14eb:  add
    IL_14ec:  stloc.s    V_4
    IL_14ee:  ldelem.i4
    IL_14ef:  stloc.s    V_6
    IL_14f1:  ldarg.0
    IL_14f2:  ldc.i4.s   15
    IL_14f4:  ldloc.s    V_6
    IL_14f6:  ldloc.s    V_0
    IL_14f8:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_14fd:  ldloc.s    V_3
    IL_14ff:  ldloc.s    V_2
    IL_1501:  ldc.i4.1
    IL_1502:  sub
    IL_1503:  dup
    IL_1504:  stloc.s    V_2
    IL_1506:  ldloc.s    V_6
    IL_1508:  stelem.i4
    IL_1509:  ldloc.s    V_3
    IL_150b:  ldloc.s    V_2
    IL_150d:  ldc.i4.1
    IL_150e:  sub
    IL_150f:  dup
    IL_1510:  stloc.s    V_2
    IL_1512:  ldc.i4.s   8
    IL_1514:  stelem.i4
    IL_1515:  br         IL_177e

    IL_151a:  ldloc.s    V_0
    IL_151c:  ldloc.s    V_10
    IL_151e:  bge        IL_1cba

    IL_1523:  ldloc.s    V_1
    IL_1525:  ldloc.s    V_0
    IL_1527:  dup
    IL_1528:  ldc.i4.s   1
    IL_152a:  add
    IL_152b:  stloc.s    V_0
    IL_152d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1532:  ldc.i4.s   64
    IL_1534:  bne.un     IL_1cba

    IL_1539:  ldloc.s    V_10
    IL_153b:  ldloc.s    V_0
    IL_153d:  sub
    IL_153e:  dup
    IL_153f:  stloc.s    V_7
    IL_1541:  ldc.i4.s   1
    IL_1543:  add
    IL_1544:  stloc.s    V_6
    IL_1546:  ldloc.s    V_6
    IL_1548:  ldc.i4.s   1
    IL_154a:  sub
    IL_154b:  dup
    IL_154c:  stloc.s    V_6
    IL_154e:  ldc.i4.s   0
    IL_1550:  ble        IL_157a

    IL_1555:  ldloc.s    V_1
    IL_1557:  ldloc.s    V_0
    IL_1559:  dup
    IL_155a:  ldc.i4.s   1
    IL_155c:  add
    IL_155d:  stloc.s    V_0
    IL_155f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1564:  ldstr      bytearray (00 00 04 00 00 00 23 00 24 00 2D 00 2E 00 )       // ......#.$.-...
    IL_1569:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_156e:  brtrue     IL_1546

    IL_1573:  ldloc.s    V_0
    IL_1575:  ldc.i4.s   1
    IL_1577:  sub
    IL_1578:  stloc.s    V_0
    IL_157a:  ldloc.s    V_7
    IL_157c:  ldloc.s    V_6
    IL_157e:  ble.s      IL_15ad

    IL_1580:  ldloc.s    V_3
    IL_1582:  ldloc.s    V_2
    IL_1584:  ldc.i4.1
    IL_1585:  sub
    IL_1586:  dup
    IL_1587:  stloc.s    V_2
    IL_1589:  ldloc.s    V_7
    IL_158b:  ldloc.s    V_6
    IL_158d:  sub
    IL_158e:  ldc.i4.s   1
    IL_1590:  sub
    IL_1591:  stelem.i4
    IL_1592:  ldloc.s    V_3
    IL_1594:  ldloc.s    V_2
    IL_1596:  ldc.i4.1
    IL_1597:  sub
    IL_1598:  dup
    IL_1599:  stloc.s    V_2
    IL_159b:  ldloc.s    V_0
    IL_159d:  ldc.i4.s   1
    IL_159f:  sub
    IL_15a0:  stelem.i4
    IL_15a1:  ldloc.s    V_3
    IL_15a3:  ldloc.s    V_2
    IL_15a5:  ldc.i4.1
    IL_15a6:  sub
    IL_15a7:  dup
    IL_15a8:  stloc.s    V_2
    IL_15aa:  ldc.i4.s   39
    IL_15ac:  stelem.i4
    IL_15ad:  ldloc.s    V_5
    IL_15af:  ldloc.s    V_4
    IL_15b1:  ldc.i4.1
    IL_15b2:  sub
    IL_15b3:  dup
    IL_15b4:  stloc.s    V_4
    IL_15b6:  ldloc.s    V_0
    IL_15b8:  stelem.i4
    IL_15b9:  ldloc.s    V_3
    IL_15bb:  ldloc.s    V_2
    IL_15bd:  ldc.i4.1
    IL_15be:  sub
    IL_15bf:  dup
    IL_15c0:  stloc.s    V_2
    IL_15c2:  ldc.i4.s   1
    IL_15c4:  stelem.i4
    IL_15c5:  ldc.i4.s   1
    IL_15c7:  ldloc.s    V_10
    IL_15c9:  ldloc.s    V_0
    IL_15cb:  sub
    IL_15cc:  bgt        IL_1cba

    IL_15d1:  ldloc.s    V_0
    IL_15d3:  ldc.i4.s   1
    IL_15d5:  add
    IL_15d6:  stloc.s    V_0
    IL_15d8:  ldc.i4.s   1
    IL_15da:  stloc.s    V_6
    IL_15dc:  ldloc.s    V_1
    IL_15de:  ldloc.s    V_0
    IL_15e0:  ldloc.s    V_6
    IL_15e2:  dup
    IL_15e3:  ldc.i4.s   1
    IL_15e5:  sub
    IL_15e6:  stloc.s    V_6
    IL_15e8:  sub
    IL_15e9:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_15ee:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_15f3:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_15f8:  brfalse    IL_1cba

    IL_15fd:  ldloc.s    V_6
    IL_15ff:  ldc.i4.s   0
    IL_1601:  bgt        IL_15dc

    IL_1606:  ldloc.s    V_10
    IL_1608:  ldloc.s    V_0
    IL_160a:  sub
    IL_160b:  dup
    IL_160c:  stloc.s    V_7
    IL_160e:  ldc.i4.s   1
    IL_1610:  add
    IL_1611:  stloc.s    V_6
    IL_1613:  ldloc.s    V_6
    IL_1615:  ldc.i4.s   1
    IL_1617:  sub
    IL_1618:  dup
    IL_1619:  stloc.s    V_6
    IL_161b:  ldc.i4.s   0
    IL_161d:  ble        IL_1647

    IL_1622:  ldloc.s    V_1
    IL_1624:  ldloc.s    V_0
    IL_1626:  dup
    IL_1627:  ldc.i4.s   1
    IL_1629:  add
    IL_162a:  stloc.s    V_0
    IL_162c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1631:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_1636:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_163b:  brtrue     IL_1613

    IL_1640:  ldloc.s    V_0
    IL_1642:  ldc.i4.s   1
    IL_1644:  sub
    IL_1645:  stloc.s    V_0
    IL_1647:  ldloc.s    V_7
    IL_1649:  ldloc.s    V_6
    IL_164b:  ble.s      IL_167a

    IL_164d:  ldloc.s    V_3
    IL_164f:  ldloc.s    V_2
    IL_1651:  ldc.i4.1
    IL_1652:  sub
    IL_1653:  dup
    IL_1654:  stloc.s    V_2
    IL_1656:  ldloc.s    V_7
    IL_1658:  ldloc.s    V_6
    IL_165a:  sub
    IL_165b:  ldc.i4.s   1
    IL_165d:  sub
    IL_165e:  stelem.i4
    IL_165f:  ldloc.s    V_3
    IL_1661:  ldloc.s    V_2
    IL_1663:  ldc.i4.1
    IL_1664:  sub
    IL_1665:  dup
    IL_1666:  stloc.s    V_2
    IL_1668:  ldloc.s    V_0
    IL_166a:  ldc.i4.s   1
    IL_166c:  sub
    IL_166d:  stelem.i4
    IL_166e:  ldloc.s    V_3
    IL_1670:  ldloc.s    V_2
    IL_1672:  ldc.i4.1
    IL_1673:  sub
    IL_1674:  dup
    IL_1675:  stloc.s    V_2
    IL_1677:  ldc.i4.s   40
    IL_1679:  stelem.i4
    IL_167a:  ldloc.s    V_0
    IL_167c:  ldloc.s    V_10
    IL_167e:  bge        IL_1cba

    IL_1683:  ldloc.s    V_1
    IL_1685:  ldloc.s    V_0
    IL_1687:  dup
    IL_1688:  ldc.i4.s   1
    IL_168a:  add
    IL_168b:  stloc.s    V_0
    IL_168d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1692:  ldc.i4.s   46
    IL_1694:  bne.un     IL_1cba

    IL_1699:  ldloc.s    V_5
    IL_169b:  ldloc.s    V_4
    IL_169d:  dup
    IL_169e:  ldc.i4.1
    IL_169f:  add
    IL_16a0:  stloc.s    V_4
    IL_16a2:  ldelem.i4
    IL_16a3:  stloc.s    V_6
    IL_16a5:  ldarg.0
    IL_16a6:  ldc.i4.s   16
    IL_16a8:  ldloc.s    V_6
    IL_16aa:  ldloc.s    V_0
    IL_16ac:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_16b1:  ldloc.s    V_3
    IL_16b3:  ldloc.s    V_2
    IL_16b5:  ldc.i4.1
    IL_16b6:  sub
    IL_16b7:  dup
    IL_16b8:  stloc.s    V_2
    IL_16ba:  ldloc.s    V_6
    IL_16bc:  stelem.i4
    IL_16bd:  ldloc.s    V_3
    IL_16bf:  ldloc.s    V_2
    IL_16c1:  ldc.i4.1
    IL_16c2:  sub
    IL_16c3:  dup
    IL_16c4:  stloc.s    V_2
    IL_16c6:  ldc.i4.s   8
    IL_16c8:  stelem.i4
    IL_16c9:  ldc.i4.s   1
    IL_16cb:  ldloc.s    V_10
    IL_16cd:  ldloc.s    V_0
    IL_16cf:  sub
    IL_16d0:  bgt        IL_1cba

    IL_16d5:  ldloc.s    V_0
    IL_16d7:  ldc.i4.s   1
    IL_16d9:  add
    IL_16da:  stloc.s    V_0
    IL_16dc:  ldc.i4.s   1
    IL_16de:  stloc.s    V_6
    IL_16e0:  ldloc.s    V_1
    IL_16e2:  ldloc.s    V_0
    IL_16e4:  ldloc.s    V_6
    IL_16e6:  dup
    IL_16e7:  ldc.i4.s   1
    IL_16e9:  sub
    IL_16ea:  stloc.s    V_6
    IL_16ec:  sub
    IL_16ed:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_16f2:  ldstr      bytearray (00 00 04 00 09 00 2B 00 2C 00 2E 00 2F 00 00 00   // ......+.,.../...
                                    02 00 04 00 05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_16f7:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_16fc:  brfalse    IL_1cba

    IL_1701:  ldloc.s    V_6
    IL_1703:  ldc.i4.s   0
    IL_1705:  bgt        IL_16e0

    IL_170a:  ldloc.s    V_10
    IL_170c:  ldloc.s    V_0
    IL_170e:  sub
    IL_170f:  dup
    IL_1710:  stloc.s    V_7
    IL_1712:  ldc.i4.s   1
    IL_1714:  add
    IL_1715:  stloc.s    V_6
    IL_1717:  ldloc.s    V_6
    IL_1719:  ldc.i4.s   1
    IL_171b:  sub
    IL_171c:  dup
    IL_171d:  stloc.s    V_6
    IL_171f:  ldc.i4.s   0
    IL_1721:  ble        IL_174b

    IL_1726:  ldloc.s    V_1
    IL_1728:  ldloc.s    V_0
    IL_172a:  dup
    IL_172b:  ldc.i4.s   1
    IL_172d:  add
    IL_172e:  stloc.s    V_0
    IL_1730:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1735:  ldstr      bytearray (00 00 04 00 09 00 2B 00 2C 00 2E 00 2F 00 00 00   // ......+.,.../...
                                    02 00 04 00 05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_173a:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_173f:  brtrue     IL_1717

    IL_1744:  ldloc.s    V_0
    IL_1746:  ldc.i4.s   1
    IL_1748:  sub
    IL_1749:  stloc.s    V_0
    IL_174b:  ldloc.s    V_7
    IL_174d:  ldloc.s    V_6
    IL_174f:  ble.s      IL_177e

    IL_1751:  ldloc.s    V_3
    IL_1753:  ldloc.s    V_2
    IL_1755:  ldc.i4.1
    IL_1756:  sub
    IL_1757:  dup
    IL_1758:  stloc.s    V_2
    IL_175a:  ldloc.s    V_7
    IL_175c:  ldloc.s    V_6
    IL_175e:  sub
    IL_175f:  ldc.i4.s   1
    IL_1761:  sub
    IL_1762:  stelem.i4
    IL_1763:  ldloc.s    V_3
    IL_1765:  ldloc.s    V_2
    IL_1767:  ldc.i4.1
    IL_1768:  sub
    IL_1769:  dup
    IL_176a:  stloc.s    V_2
    IL_176c:  ldloc.s    V_0
    IL_176e:  ldc.i4.s   1
    IL_1770:  sub
    IL_1771:  stelem.i4
    IL_1772:  ldloc.s    V_3
    IL_1774:  ldloc.s    V_2
    IL_1776:  ldc.i4.1
    IL_1777:  sub
    IL_1778:  dup
    IL_1779:  stloc.s    V_2
    IL_177b:  ldc.i4.s   41
    IL_177d:  stelem.i4
    IL_177e:  ldloc.s    V_5
    IL_1780:  ldloc.s    V_4
    IL_1782:  dup
    IL_1783:  ldc.i4.1
    IL_1784:  add
    IL_1785:  stloc.s    V_4
    IL_1787:  ldelem.i4
    IL_1788:  stloc.s    V_6
    IL_178a:  ldarg.0
    IL_178b:  ldc.i4.s   13
    IL_178d:  ldloc.s    V_6
    IL_178f:  ldloc.s    V_0
    IL_1791:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_1796:  ldloc.s    V_3
    IL_1798:  ldloc.s    V_2
    IL_179a:  ldc.i4.1
    IL_179b:  sub
    IL_179c:  dup
    IL_179d:  stloc.s    V_2
    IL_179f:  ldloc.s    V_6
    IL_17a1:  stelem.i4
    IL_17a2:  ldloc.s    V_3
    IL_17a4:  ldloc.s    V_2
    IL_17a6:  ldc.i4.1
    IL_17a7:  sub
    IL_17a8:  dup
    IL_17a9:  stloc.s    V_2
    IL_17ab:  ldc.i4.s   8
    IL_17ad:  stelem.i4
    IL_17ae:  ldloc.s    V_5
    IL_17b0:  ldloc.s    V_4
    IL_17b2:  dup
    IL_17b3:  ldc.i4.1
    IL_17b4:  add
    IL_17b5:  stloc.s    V_4
    IL_17b7:  ldelem.i4
    IL_17b8:  stloc.s    V_6
    IL_17ba:  ldloc.s    V_5
    IL_17bc:  ldloc.s    V_4
    IL_17be:  dup
    IL_17bf:  ldc.i4.1
    IL_17c0:  add
    IL_17c1:  stloc.s    V_4
    IL_17c3:  ldelem.i4
    IL_17c4:  dup
    IL_17c5:  stloc.s    V_7
    IL_17c7:  ldloc.s    V_3
    IL_17c9:  ldloc.s    V_2
    IL_17cb:  ldc.i4.1
    IL_17cc:  sub
    IL_17cd:  dup
    IL_17ce:  stloc.s    V_2
    IL_17d0:  ldloc.s    V_7
    IL_17d2:  stelem.i4
    IL_17d3:  ldloc.s    V_0
    IL_17d5:  bne.un.s   IL_17dd

    IL_17d7:  ldloc.s    V_6
    IL_17d9:  ldc.i4.s   0
    IL_17db:  bge.s      IL_1830

    IL_17dd:  ldloc.s    V_6
    IL_17df:  ldc.i4.s   1
    IL_17e1:  bge.s      IL_1830

    IL_17e3:  ldloc.s    V_5
    IL_17e5:  ldloc.s    V_4
    IL_17e7:  ldc.i4.1
    IL_17e8:  sub
    IL_17e9:  dup
    IL_17ea:  stloc.s    V_4
    IL_17ec:  ldloc.s    V_0
    IL_17ee:  stelem.i4
    IL_17ef:  ldloc.s    V_5
    IL_17f1:  ldloc.s    V_4
    IL_17f3:  ldc.i4.1
    IL_17f4:  sub
    IL_17f5:  dup
    IL_17f6:  stloc.s    V_4
    IL_17f8:  ldloc.s    V_6
    IL_17fa:  ldc.i4.s   1
    IL_17fc:  add
    IL_17fd:  stelem.i4
    IL_17fe:  ldloc.s    V_3
    IL_1800:  ldloc.s    V_2
    IL_1802:  ldc.i4.1
    IL_1803:  sub
    IL_1804:  dup
    IL_1805:  stloc.s    V_2
    IL_1807:  ldc.i4.s   42
    IL_1809:  stelem.i4
    IL_180a:  ldloc.s    V_2
    IL_180c:  ldc.i4     0x188
    IL_1811:  ble.s      IL_181f

    IL_1813:  ldloc.s    V_4
    IL_1815:  ldc.i4     0x126
    IL_181a:  bgt        IL_128b

    IL_181f:  ldloc.s    V_3
    IL_1821:  ldloc.s    V_2
    IL_1823:  ldc.i4.1
    IL_1824:  sub
    IL_1825:  dup
    IL_1826:  stloc.s    V_2
    IL_1828:  ldc.i4.s   43
    IL_182a:  stelem.i4
    IL_182b:  br         IL_1cba

    IL_1830:  ldloc.s    V_3
    IL_1832:  ldloc.s    V_2
    IL_1834:  ldc.i4.1
    IL_1835:  sub
    IL_1836:  dup
    IL_1837:  stloc.s    V_2
    IL_1839:  ldloc.s    V_6
    IL_183b:  stelem.i4
    IL_183c:  ldloc.s    V_3
    IL_183e:  ldloc.s    V_2
    IL_1840:  ldc.i4.1
    IL_1841:  sub
    IL_1842:  dup
    IL_1843:  stloc.s    V_2
    IL_1845:  ldc.i4.s   11
    IL_1847:  stelem.i4
    IL_1848:  ldloc.s    V_5
    IL_184a:  ldloc.s    V_4
    IL_184c:  dup
    IL_184d:  ldc.i4.1
    IL_184e:  add
    IL_184f:  stloc.s    V_4
    IL_1851:  ldelem.i4
    IL_1852:  stloc.s    V_6
    IL_1854:  ldarg.0
    IL_1855:  ldc.i4.s   11
    IL_1857:  ldloc.s    V_6
    IL_1859:  ldloc.s    V_0
    IL_185b:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_1860:  ldloc.s    V_3
    IL_1862:  ldloc.s    V_2
    IL_1864:  ldc.i4.1
    IL_1865:  sub
    IL_1866:  dup
    IL_1867:  stloc.s    V_2
    IL_1869:  ldloc.s    V_6
    IL_186b:  stelem.i4
    IL_186c:  ldloc.s    V_3
    IL_186e:  ldloc.s    V_2
    IL_1870:  ldc.i4.1
    IL_1871:  sub
    IL_1872:  dup
    IL_1873:  stloc.s    V_2
    IL_1875:  ldc.i4.s   8
    IL_1877:  stelem.i4
    IL_1878:  ldloc.s    V_5
    IL_187a:  ldloc.s    V_4
    IL_187c:  dup
    IL_187d:  ldc.i4.1
    IL_187e:  add
    IL_187f:  stloc.s    V_4
    IL_1881:  ldelem.i4
    IL_1882:  dup
    IL_1883:  stloc.s    V_6
    IL_1885:  ldloc.s    V_3
    IL_1887:  ldloc.s    V_2
    IL_1889:  ldc.i4.1
    IL_188a:  sub
    IL_188b:  dup
    IL_188c:  stloc.s    V_2
    IL_188e:  ldloc.s    V_6
    IL_1890:  stelem.i4
    IL_1891:  ldloc.s    V_0
    IL_1893:  beq.s      IL_18df

    IL_1895:  ldloc.s    V_3
    IL_1897:  ldloc.s    V_2
    IL_1899:  ldc.i4.1
    IL_189a:  sub
    IL_189b:  dup
    IL_189c:  stloc.s    V_2
    IL_189e:  ldloc.s    V_0
    IL_18a0:  stelem.i4
    IL_18a1:  ldloc.s    V_5
    IL_18a3:  ldloc.s    V_4
    IL_18a5:  ldc.i4.1
    IL_18a6:  sub
    IL_18a7:  dup
    IL_18a8:  stloc.s    V_4
    IL_18aa:  ldloc.s    V_0
    IL_18ac:  stelem.i4
    IL_18ad:  ldloc.s    V_3
    IL_18af:  ldloc.s    V_2
    IL_18b1:  ldc.i4.1
    IL_18b2:  sub
    IL_18b3:  dup
    IL_18b4:  stloc.s    V_2
    IL_18b6:  ldc.i4.s   44
    IL_18b8:  stelem.i4
    IL_18b9:  ldloc.s    V_2
    IL_18bb:  ldc.i4     0x188
    IL_18c0:  ble.s      IL_18ce

    IL_18c2:  ldloc.s    V_4
    IL_18c4:  ldc.i4     0x126
    IL_18c9:  bgt        IL_1120

    IL_18ce:  ldloc.s    V_3
    IL_18d0:  ldloc.s    V_2
    IL_18d2:  ldc.i4.1
    IL_18d3:  sub
    IL_18d4:  dup
    IL_18d5:  stloc.s    V_2
    IL_18d7:  ldc.i4.s   45
    IL_18d9:  stelem.i4
    IL_18da:  br         IL_1cba

    IL_18df:  ldloc.s    V_3
    IL_18e1:  ldloc.s    V_2
    IL_18e3:  ldc.i4.1
    IL_18e4:  sub
    IL_18e5:  dup
    IL_18e6:  stloc.s    V_2
    IL_18e8:  ldc.i4.s   46
    IL_18ea:  stelem.i4
    IL_18eb:  ldloc.s    V_5
    IL_18ed:  ldloc.s    V_4
    IL_18ef:  dup
    IL_18f0:  ldc.i4.1
    IL_18f1:  add
    IL_18f2:  stloc.s    V_4
    IL_18f4:  ldelem.i4
    IL_18f5:  stloc.s    V_6
    IL_18f7:  ldarg.0
    IL_18f8:  ldc.i4.s   10
    IL_18fa:  ldloc.s    V_6
    IL_18fc:  ldloc.s    V_0
    IL_18fe:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_1903:  ldloc.s    V_3
    IL_1905:  ldloc.s    V_2
    IL_1907:  ldc.i4.1
    IL_1908:  sub
    IL_1909:  dup
    IL_190a:  stloc.s    V_2
    IL_190c:  ldloc.s    V_6
    IL_190e:  stelem.i4
    IL_190f:  ldloc.s    V_3
    IL_1911:  ldloc.s    V_2
    IL_1913:  ldc.i4.1
    IL_1914:  sub
    IL_1915:  dup
    IL_1916:  stloc.s    V_2
    IL_1918:  ldc.i4.s   8
    IL_191a:  stelem.i4
    IL_191b:  ldloc.s    V_5
    IL_191d:  ldloc.s    V_4
    IL_191f:  dup
    IL_1920:  ldc.i4.1
    IL_1921:  add
    IL_1922:  stloc.s    V_4
    IL_1924:  ldelem.i4
    IL_1925:  stloc.s    V_6
    IL_1927:  ldloc.s    V_5
    IL_1929:  ldloc.s    V_4
    IL_192b:  dup
    IL_192c:  ldc.i4.1
    IL_192d:  add
    IL_192e:  stloc.s    V_4
    IL_1930:  ldelem.i4
    IL_1931:  dup
    IL_1932:  stloc.s    V_7
    IL_1934:  ldloc.s    V_3
    IL_1936:  ldloc.s    V_2
    IL_1938:  ldc.i4.1
    IL_1939:  sub
    IL_193a:  dup
    IL_193b:  stloc.s    V_2
    IL_193d:  ldloc.s    V_7
    IL_193f:  stelem.i4
    IL_1940:  ldloc.s    V_0
    IL_1942:  bne.un.s   IL_194a

    IL_1944:  ldloc.s    V_6
    IL_1946:  ldc.i4.s   0
    IL_1948:  bge.s      IL_199d

    IL_194a:  ldloc.s    V_6
    IL_194c:  ldc.i4.s   1
    IL_194e:  bge.s      IL_199d

    IL_1950:  ldloc.s    V_5
    IL_1952:  ldloc.s    V_4
    IL_1954:  ldc.i4.1
    IL_1955:  sub
    IL_1956:  dup
    IL_1957:  stloc.s    V_4
    IL_1959:  ldloc.s    V_0
    IL_195b:  stelem.i4
    IL_195c:  ldloc.s    V_5
    IL_195e:  ldloc.s    V_4
    IL_1960:  ldc.i4.1
    IL_1961:  sub
    IL_1962:  dup
    IL_1963:  stloc.s    V_4
    IL_1965:  ldloc.s    V_6
    IL_1967:  ldc.i4.s   1
    IL_1969:  add
    IL_196a:  stelem.i4
    IL_196b:  ldloc.s    V_3
    IL_196d:  ldloc.s    V_2
    IL_196f:  ldc.i4.1
    IL_1970:  sub
    IL_1971:  dup
    IL_1972:  stloc.s    V_2
    IL_1974:  ldc.i4.s   47
    IL_1976:  stelem.i4
    IL_1977:  ldloc.s    V_2
    IL_1979:  ldc.i4     0x188
    IL_197e:  ble.s      IL_198c

    IL_1980:  ldloc.s    V_4
    IL_1982:  ldc.i4     0x126
    IL_1987:  bgt        IL_10cc

    IL_198c:  ldloc.s    V_3
    IL_198e:  ldloc.s    V_2
    IL_1990:  ldc.i4.1
    IL_1991:  sub
    IL_1992:  dup
    IL_1993:  stloc.s    V_2
    IL_1995:  ldc.i4.s   48
    IL_1997:  stelem.i4
    IL_1998:  br         IL_1cba

    IL_199d:  ldloc.s    V_3
    IL_199f:  ldloc.s    V_2
    IL_19a1:  ldc.i4.1
    IL_19a2:  sub
    IL_19a3:  dup
    IL_19a4:  stloc.s    V_2
    IL_19a6:  ldloc.s    V_6
    IL_19a8:  stelem.i4
    IL_19a9:  ldloc.s    V_3
    IL_19ab:  ldloc.s    V_2
    IL_19ad:  ldc.i4.1
    IL_19ae:  sub
    IL_19af:  dup
    IL_19b0:  stloc.s    V_2
    IL_19b2:  ldc.i4.s   11
    IL_19b4:  stelem.i4
    IL_19b5:  ldloc.s    V_0
    IL_19b7:  ldloc.s    V_10
    IL_19b9:  bge        IL_1cba

    IL_19be:  ldloc.s    V_1
    IL_19c0:  ldloc.s    V_0
    IL_19c2:  dup
    IL_19c3:  ldc.i4.s   1
    IL_19c5:  add
    IL_19c6:  stloc.s    V_0
    IL_19c8:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_19cd:  ldc.i4.s   36
    IL_19cf:  bne.un     IL_1cba

    IL_19d4:  ldloc.s    V_5
    IL_19d6:  ldloc.s    V_4
    IL_19d8:  dup
    IL_19d9:  ldc.i4.1
    IL_19da:  add
    IL_19db:  stloc.s    V_4
    IL_19dd:  ldelem.i4
    IL_19de:  stloc.s    V_6
    IL_19e0:  ldarg.0
    IL_19e1:  ldc.i4.s   9
    IL_19e3:  ldloc.s    V_6
    IL_19e5:  ldloc.s    V_0
    IL_19e7:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_19ec:  ldloc.s    V_3
    IL_19ee:  ldloc.s    V_2
    IL_19f0:  ldc.i4.1
    IL_19f1:  sub
    IL_19f2:  dup
    IL_19f3:  stloc.s    V_2
    IL_19f5:  ldloc.s    V_6
    IL_19f7:  stelem.i4
    IL_19f8:  ldloc.s    V_3
    IL_19fa:  ldloc.s    V_2
    IL_19fc:  ldc.i4.1
    IL_19fd:  sub
    IL_19fe:  dup
    IL_19ff:  stloc.s    V_2
    IL_1a01:  ldc.i4.s   8
    IL_1a03:  stelem.i4
    IL_1a04:  ldloc.s    V_5
    IL_1a06:  ldloc.s    V_4
    IL_1a08:  dup
    IL_1a09:  ldc.i4.1
    IL_1a0a:  add
    IL_1a0b:  stloc.s    V_4
    IL_1a0d:  ldelem.i4
    IL_1a0e:  stloc.s    V_6
    IL_1a10:  ldarg.0
    IL_1a11:  ldc.i4.s   1
    IL_1a13:  ldloc.s    V_6
    IL_1a15:  ldloc.s    V_0
    IL_1a17:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_1a1c:  ldloc.s    V_3
    IL_1a1e:  ldloc.s    V_2
    IL_1a20:  ldc.i4.1
    IL_1a21:  sub
    IL_1a22:  dup
    IL_1a23:  stloc.s    V_2
    IL_1a25:  ldloc.s    V_6
    IL_1a27:  stelem.i4
    IL_1a28:  ldloc.s    V_3
    IL_1a2a:  ldloc.s    V_2
    IL_1a2c:  ldc.i4.1
    IL_1a2d:  sub
    IL_1a2e:  dup
    IL_1a2f:  stloc.s    V_2
    IL_1a31:  ldc.i4.s   8
    IL_1a33:  stelem.i4
    IL_1a34:  br         IL_1c81

    IL_1a39:  ldloc.s    V_3
    IL_1a3b:  ldloc.s    V_2
    IL_1a3d:  ldc.i4.1
    IL_1a3e:  sub
    IL_1a3f:  dup
    IL_1a40:  stloc.s    V_2
    IL_1a42:  ldloc.s    V_0
    IL_1a44:  stelem.i4
    IL_1a45:  ldloc.s    V_3
    IL_1a47:  ldloc.s    V_2
    IL_1a49:  ldc.i4.1
    IL_1a4a:  sub
    IL_1a4b:  dup
    IL_1a4c:  stloc.s    V_2
    IL_1a4e:  ldc.i4.s   49
    IL_1a50:  stelem.i4
    IL_1a51:  ldc.i4.s   2
    IL_1a53:  ldloc.s    V_10
    IL_1a55:  ldloc.s    V_0
    IL_1a57:  sub
    IL_1a58:  bgt        IL_1cba

    IL_1a5d:  ldloc.s    V_1
    IL_1a5f:  ldloc.s    V_0
    IL_1a61:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1a66:  ldc.i4.s   125
    IL_1a68:  bne.un     IL_1cba

    IL_1a6d:  ldloc.s    V_1
    IL_1a6f:  ldloc.s    V_0
    IL_1a71:  ldc.i4.s   1
    IL_1a73:  add
    IL_1a74:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1a79:  ldc.i4.s   58
    IL_1a7b:  bne.un     IL_1cba

    IL_1a80:  ldloc.s    V_0
    IL_1a82:  ldc.i4.s   2
    IL_1a84:  add
    IL_1a85:  stloc.s    V_0
    IL_1a87:  ldc.i4.s   1
    IL_1a89:  ldloc.s    V_10
    IL_1a8b:  ldloc.s    V_0
    IL_1a8d:  sub
    IL_1a8e:  bgt        IL_1cba

    IL_1a93:  ldloc.s    V_0
    IL_1a95:  ldc.i4.s   1
    IL_1a97:  add
    IL_1a98:  stloc.s    V_0
    IL_1a9a:  ldc.i4.s   1
    IL_1a9c:  stloc.s    V_6
    IL_1a9e:  ldloc.s    V_1
    IL_1aa0:  ldloc.s    V_0
    IL_1aa2:  ldloc.s    V_6
    IL_1aa4:  dup
    IL_1aa5:  ldc.i4.s   1
    IL_1aa7:  sub
    IL_1aa8:  stloc.s    V_6
    IL_1aaa:  sub
    IL_1aab:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1ab0:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_1ab5:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1aba:  brfalse    IL_1cba

    IL_1abf:  ldloc.s    V_6
    IL_1ac1:  ldc.i4.s   0
    IL_1ac3:  bgt        IL_1a9e

    IL_1ac8:  ldloc.s    V_10
    IL_1aca:  ldloc.s    V_0
    IL_1acc:  sub
    IL_1acd:  dup
    IL_1ace:  stloc.s    V_7
    IL_1ad0:  ldc.i4.s   1
    IL_1ad2:  add
    IL_1ad3:  stloc.s    V_6
    IL_1ad5:  ldloc.s    V_6
    IL_1ad7:  ldc.i4.s   1
    IL_1ad9:  sub
    IL_1ada:  dup
    IL_1adb:  stloc.s    V_6
    IL_1add:  ldc.i4.s   0
    IL_1adf:  ble        IL_1b09

    IL_1ae4:  ldloc.s    V_1
    IL_1ae6:  ldloc.s    V_0
    IL_1ae8:  dup
    IL_1ae9:  ldc.i4.s   1
    IL_1aeb:  add
    IL_1aec:  stloc.s    V_0
    IL_1aee:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1af3:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_1af8:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1afd:  brtrue     IL_1ad5

    IL_1b02:  ldloc.s    V_0
    IL_1b04:  ldc.i4.s   1
    IL_1b06:  sub
    IL_1b07:  stloc.s    V_0
    IL_1b09:  ldloc.s    V_7
    IL_1b0b:  ldloc.s    V_6
    IL_1b0d:  ble.s      IL_1b3c

    IL_1b0f:  ldloc.s    V_3
    IL_1b11:  ldloc.s    V_2
    IL_1b13:  ldc.i4.1
    IL_1b14:  sub
    IL_1b15:  dup
    IL_1b16:  stloc.s    V_2
    IL_1b18:  ldloc.s    V_7
    IL_1b1a:  ldloc.s    V_6
    IL_1b1c:  sub
    IL_1b1d:  ldc.i4.s   1
    IL_1b1f:  sub
    IL_1b20:  stelem.i4
    IL_1b21:  ldloc.s    V_3
    IL_1b23:  ldloc.s    V_2
    IL_1b25:  ldc.i4.1
    IL_1b26:  sub
    IL_1b27:  dup
    IL_1b28:  stloc.s    V_2
    IL_1b2a:  ldloc.s    V_0
    IL_1b2c:  ldc.i4.s   1
    IL_1b2e:  sub
    IL_1b2f:  stelem.i4
    IL_1b30:  ldloc.s    V_3
    IL_1b32:  ldloc.s    V_2
    IL_1b34:  ldc.i4.1
    IL_1b35:  sub
    IL_1b36:  dup
    IL_1b37:  stloc.s    V_2
    IL_1b39:  ldc.i4.s   50
    IL_1b3b:  stelem.i4
    IL_1b3c:  ldc.i4.s   2
    IL_1b3e:  ldloc.s    V_10
    IL_1b40:  ldloc.s    V_0
    IL_1b42:  sub
    IL_1b43:  bgt        IL_1cba

    IL_1b48:  ldloc.s    V_1
    IL_1b4a:  ldloc.s    V_0
    IL_1b4c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1b51:  ldc.i4.s   58
    IL_1b53:  bne.un     IL_1cba

    IL_1b58:  ldloc.s    V_1
    IL_1b5a:  ldloc.s    V_0
    IL_1b5c:  ldc.i4.s   1
    IL_1b5e:  add
    IL_1b5f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1b64:  ldc.i4.s   123
    IL_1b66:  bne.un     IL_1cba

    IL_1b6b:  ldloc.s    V_0
    IL_1b6d:  ldc.i4.s   2
    IL_1b6f:  add
    IL_1b70:  stloc.s    V_0
    IL_1b72:  br         IL_1c81

    IL_1b77:  ldc.i4.s   2
    IL_1b79:  ldloc.s    V_10
    IL_1b7b:  ldloc.s    V_0
    IL_1b7d:  sub
    IL_1b7e:  bgt        IL_1cba

    IL_1b83:  ldloc.s    V_1
    IL_1b85:  ldloc.s    V_0
    IL_1b87:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1b8c:  ldc.i4.s   125
    IL_1b8e:  bne.un     IL_1cba

    IL_1b93:  ldloc.s    V_1
    IL_1b95:  ldloc.s    V_0
    IL_1b97:  ldc.i4.s   1
    IL_1b99:  add
    IL_1b9a:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1b9f:  ldc.i4.s   58
    IL_1ba1:  bne.un     IL_1cba

    IL_1ba6:  ldloc.s    V_0
    IL_1ba8:  ldc.i4.s   2
    IL_1baa:  add
    IL_1bab:  stloc.s    V_0
    IL_1bad:  ldc.i4.s   1
    IL_1baf:  ldloc.s    V_10
    IL_1bb1:  ldloc.s    V_0
    IL_1bb3:  sub
    IL_1bb4:  bgt        IL_1cba

    IL_1bb9:  ldloc.s    V_0
    IL_1bbb:  ldc.i4.s   1
    IL_1bbd:  add
    IL_1bbe:  stloc.s    V_0
    IL_1bc0:  ldc.i4.s   1
    IL_1bc2:  stloc.s    V_6
    IL_1bc4:  ldloc.s    V_1
    IL_1bc6:  ldloc.s    V_0
    IL_1bc8:  ldloc.s    V_6
    IL_1bca:  dup
    IL_1bcb:  ldc.i4.s   1
    IL_1bcd:  sub
    IL_1bce:  stloc.s    V_6
    IL_1bd0:  sub
    IL_1bd1:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1bd6:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_1bdb:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1be0:  brfalse    IL_1cba

    IL_1be5:  ldloc.s    V_6
    IL_1be7:  ldc.i4.s   0
    IL_1be9:  bgt        IL_1bc4

    IL_1bee:  ldloc.s    V_10
    IL_1bf0:  ldloc.s    V_0
    IL_1bf2:  sub
    IL_1bf3:  dup
    IL_1bf4:  stloc.s    V_7
    IL_1bf6:  ldc.i4.s   1
    IL_1bf8:  add
    IL_1bf9:  stloc.s    V_6
    IL_1bfb:  ldloc.s    V_6
    IL_1bfd:  ldc.i4.s   1
    IL_1bff:  sub
    IL_1c00:  dup
    IL_1c01:  stloc.s    V_6
    IL_1c03:  ldc.i4.s   0
    IL_1c05:  ble        IL_1c2f

    IL_1c0a:  ldloc.s    V_1
    IL_1c0c:  ldloc.s    V_0
    IL_1c0e:  dup
    IL_1c0f:  ldc.i4.s   1
    IL_1c11:  add
    IL_1c12:  stloc.s    V_0
    IL_1c14:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1c19:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_1c1e:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_1c23:  brtrue     IL_1bfb

    IL_1c28:  ldloc.s    V_0
    IL_1c2a:  ldc.i4.s   1
    IL_1c2c:  sub
    IL_1c2d:  stloc.s    V_0
    IL_1c2f:  ldloc.s    V_7
    IL_1c31:  ldloc.s    V_6
    IL_1c33:  ble.s      IL_1c62

    IL_1c35:  ldloc.s    V_3
    IL_1c37:  ldloc.s    V_2
    IL_1c39:  ldc.i4.1
    IL_1c3a:  sub
    IL_1c3b:  dup
    IL_1c3c:  stloc.s    V_2
    IL_1c3e:  ldloc.s    V_7
    IL_1c40:  ldloc.s    V_6
    IL_1c42:  sub
    IL_1c43:  ldc.i4.s   1
    IL_1c45:  sub
    IL_1c46:  stelem.i4
    IL_1c47:  ldloc.s    V_3
    IL_1c49:  ldloc.s    V_2
    IL_1c4b:  ldc.i4.1
    IL_1c4c:  sub
    IL_1c4d:  dup
    IL_1c4e:  stloc.s    V_2
    IL_1c50:  ldloc.s    V_0
    IL_1c52:  ldc.i4.s   1
    IL_1c54:  sub
    IL_1c55:  stelem.i4
    IL_1c56:  ldloc.s    V_3
    IL_1c58:  ldloc.s    V_2
    IL_1c5a:  ldc.i4.1
    IL_1c5b:  sub
    IL_1c5c:  dup
    IL_1c5d:  stloc.s    V_2
    IL_1c5f:  ldc.i4.s   51
    IL_1c61:  stelem.i4
    IL_1c62:  ldloc.s    V_0
    IL_1c64:  ldloc.s    V_10
    IL_1c66:  bge        IL_1cba

    IL_1c6b:  ldloc.s    V_1
    IL_1c6d:  ldloc.s    V_0
    IL_1c6f:  dup
    IL_1c70:  ldc.i4.s   1
    IL_1c72:  add
    IL_1c73:  stloc.s    V_0
    IL_1c75:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_1c7a:  ldc.i4.s   36
    IL_1c7c:  bne.un     IL_1cba

    IL_1c81:  ldloc.s    V_5
    IL_1c83:  ldloc.s    V_4
    IL_1c85:  dup
    IL_1c86:  ldc.i4.1
    IL_1c87:  add
    IL_1c88:  stloc.s    V_4
    IL_1c8a:  ldelem.i4
    IL_1c8b:  stloc.s    V_6
    IL_1c8d:  ldarg.0
    IL_1c8e:  ldc.i4.s   0
    IL_1c90:  ldloc.s    V_6
    IL_1c92:  ldloc.s    V_0
    IL_1c94:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_1c99:  ldloc.s    V_3
    IL_1c9b:  ldloc.s    V_2
    IL_1c9d:  ldc.i4.1
    IL_1c9e:  sub
    IL_1c9f:  dup
    IL_1ca0:  stloc.s    V_2
    IL_1ca2:  ldloc.s    V_6
    IL_1ca4:  stelem.i4
    IL_1ca5:  ldloc.s    V_3
    IL_1ca7:  ldloc.s    V_2
    IL_1ca9:  ldc.i4.1
    IL_1caa:  sub
    IL_1cab:  dup
    IL_1cac:  stloc.s    V_2
    IL_1cae:  ldc.i4.s   8
    IL_1cb0:  stelem.i4
    IL_1cb1:  ldarg.0
    IL_1cb2:  ldloc.s    V_0
    IL_1cb4:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_1cb9:  ret

    IL_1cba:  ldarg.0
    IL_1cbb:  ldloc.s    V_2
    IL_1cbd:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_1cc2:  ldarg.0
    IL_1cc3:  ldloc.s    V_4
    IL_1cc5:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_1cca:  ldarg.0
    IL_1ccb:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::EnsureStorage()
    IL_1cd0:  ldarg.0
    IL_1cd1:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_1cd6:  stloc.s    V_2
    IL_1cd8:  ldarg.0
    IL_1cd9:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_1cde:  stloc.s    V_4
    IL_1ce0:  ldarg.0
    IL_1ce1:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_1ce6:  stloc.s    V_3
    IL_1ce8:  ldarg.0
    IL_1ce9:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_1cee:  stloc.s    V_5
    IL_1cf0:  ldloc.s    V_3
    IL_1cf2:  ldloc.s    V_2
    IL_1cf4:  dup
    IL_1cf5:  ldc.i4.1
    IL_1cf6:  add
    IL_1cf7:  stloc.s    V_2
    IL_1cf9:  ldelem.i4
    IL_1cfa:  switch     ( 
                          IL_1dcf,
                          IL_1de0,
                          IL_1dec,
                          IL_1dfd,
                          IL_1e0e,
                          IL_1e1f,
                          IL_1e2b,
                          IL_1e3c,
                          IL_1e8c,
                          IL_1eab,
                          IL_0138,
                          IL_1f08,
                          IL_1f39,
                          IL_1f89,
                          IL_1fd9,
                          IL_2029,
                          IL_2079,
                          IL_208a,
                          IL_20da,
                          IL_212a,
                          IL_0921,
                          IL_2187,
                          IL_080c,
                          IL_21e4,
                          IL_2234,
                          IL_0b15,
                          IL_2291,
                          IL_22a2,
                          IL_22f2,
                          IL_2303,
                          IL_2353,
                          IL_23a3,
                          IL_23f3,
                          IL_1161,
                          IL_2450,
                          IL_2461,
                          IL_24b1,
                          IL_2501,
                          IL_2512,
                          IL_2562,
                          IL_25b2,
                          IL_2602,
                          IL_2652,
                          IL_128b,
                          IL_26af,
                          IL_1120,
                          IL_26d7,
                          IL_26f0,
                          IL_10cc,
                          IL_274d,
                          IL_275e,
                          IL_27ae)
    IL_1dcf:  ldloc.s    V_3
    IL_1dd1:  ldloc.s    V_2
    IL_1dd3:  dup
    IL_1dd4:  ldc.i4.1
    IL_1dd5:  add
    IL_1dd6:  stloc.s    V_2
    IL_1dd8:  ldelem.i4
    IL_1dd9:  stloc.s    V_0
    IL_1ddb:  br         IL_1cb1

    IL_1de0:  ldloc.s    V_4
    IL_1de2:  ldc.i4.s   1
    IL_1de4:  add
    IL_1de5:  stloc.s    V_4
    IL_1de7:  br         IL_1cba

    IL_1dec:  ldloc.s    V_3
    IL_1dee:  ldloc.s    V_2
    IL_1df0:  dup
    IL_1df1:  ldc.i4.1
    IL_1df2:  add
    IL_1df3:  stloc.s    V_2
    IL_1df5:  ldelem.i4
    IL_1df6:  stloc.s    V_0
    IL_1df8:  br         IL_1a39

    IL_1dfd:  ldloc.s    V_3
    IL_1dff:  ldloc.s    V_2
    IL_1e01:  dup
    IL_1e02:  ldc.i4.1
    IL_1e03:  add
    IL_1e04:  stloc.s    V_2
    IL_1e06:  ldelem.i4
    IL_1e07:  stloc.s    V_0
    IL_1e09:  br         IL_06b4

    IL_1e0e:  ldloc.s    V_3
    IL_1e10:  ldloc.s    V_2
    IL_1e12:  dup
    IL_1e13:  ldc.i4.1
    IL_1e14:  add
    IL_1e15:  stloc.s    V_2
    IL_1e17:  ldelem.i4
    IL_1e18:  stloc.s    V_0
    IL_1e1a:  br         IL_03fc

    IL_1e1f:  ldloc.s    V_4
    IL_1e21:  ldc.i4.s   2
    IL_1e23:  add
    IL_1e24:  stloc.s    V_4
    IL_1e26:  br         IL_1cba

    IL_1e2b:  ldloc.s    V_3
    IL_1e2d:  ldloc.s    V_2
    IL_1e2f:  dup
    IL_1e30:  ldc.i4.1
    IL_1e31:  add
    IL_1e32:  stloc.s    V_2
    IL_1e34:  ldelem.i4
    IL_1e35:  stloc.s    V_0
    IL_1e37:  br         IL_0209

    IL_1e3c:  ldloc.s    V_3
    IL_1e3e:  ldloc.s    V_2
    IL_1e40:  dup
    IL_1e41:  ldc.i4.1
    IL_1e42:  add
    IL_1e43:  stloc.s    V_2
    IL_1e45:  ldelem.i4
    IL_1e46:  stloc.s    V_0
    IL_1e48:  ldloc.s    V_3
    IL_1e4a:  ldloc.s    V_2
    IL_1e4c:  dup
    IL_1e4d:  ldc.i4.1
    IL_1e4e:  add
    IL_1e4f:  stloc.s    V_2
    IL_1e51:  ldelem.i4
    IL_1e52:  stloc.s    V_6
    IL_1e54:  ldloc.s    V_6
    IL_1e56:  ldc.i4.s   0
    IL_1e58:  ble        IL_0204

    IL_1e5d:  ldloc.s    V_3
    IL_1e5f:  ldloc.s    V_2
    IL_1e61:  ldc.i4.1
    IL_1e62:  sub
    IL_1e63:  dup
    IL_1e64:  stloc.s    V_2
    IL_1e66:  ldloc.s    V_6
    IL_1e68:  ldc.i4.s   1
    IL_1e6a:  sub
    IL_1e6b:  stelem.i4
    IL_1e6c:  ldloc.s    V_3
    IL_1e6e:  ldloc.s    V_2
    IL_1e70:  ldc.i4.1
    IL_1e71:  sub
    IL_1e72:  dup
    IL_1e73:  stloc.s    V_2
    IL_1e75:  ldloc.s    V_0
    IL_1e77:  ldc.i4.s   1
    IL_1e79:  sub
    IL_1e7a:  stelem.i4
    IL_1e7b:  ldloc.s    V_3
    IL_1e7d:  ldloc.s    V_2
    IL_1e7f:  ldc.i4.1
    IL_1e80:  sub
    IL_1e81:  dup
    IL_1e82:  stloc.s    V_2
    IL_1e84:  ldc.i4.s   7
    IL_1e86:  stelem.i4
    IL_1e87:  br         IL_0204

    IL_1e8c:  ldloc.s    V_5
    IL_1e8e:  ldloc.s    V_4
    IL_1e90:  ldc.i4.1
    IL_1e91:  sub
    IL_1e92:  dup
    IL_1e93:  stloc.s    V_4
    IL_1e95:  ldloc.s    V_3
    IL_1e97:  ldloc.s    V_2
    IL_1e99:  dup
    IL_1e9a:  ldc.i4.1
    IL_1e9b:  add
    IL_1e9c:  stloc.s    V_2
    IL_1e9e:  ldelem.i4
    IL_1e9f:  stelem.i4
    IL_1ea0:  ldarg.0
    IL_1ea1:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Uncapture()
    IL_1ea6:  br         IL_1cba

    IL_1eab:  ldloc.s    V_5
    IL_1ead:  ldloc.s    V_4
    IL_1eaf:  dup
    IL_1eb0:  ldc.i4.1
    IL_1eb1:  add
    IL_1eb2:  stloc.s    V_4
    IL_1eb4:  ldelem.i4
    IL_1eb5:  ldc.i4.s   1
    IL_1eb7:  sub
    IL_1eb8:  dup
    IL_1eb9:  stloc.s    V_6
    IL_1ebb:  ldc.i4.s   0
    IL_1ebd:  blt.s      IL_1ee8

    IL_1ebf:  ldloc.s    V_5
    IL_1ec1:  ldloc.s    V_4
    IL_1ec3:  dup
    IL_1ec4:  ldc.i4.1
    IL_1ec5:  add
    IL_1ec6:  stloc.s    V_4
    IL_1ec8:  ldelem.i4
    IL_1ec9:  stloc.s    V_0
    IL_1ecb:  ldloc.s    V_3
    IL_1ecd:  ldloc.s    V_2
    IL_1ecf:  ldc.i4.1
    IL_1ed0:  sub
    IL_1ed1:  dup
    IL_1ed2:  stloc.s    V_2
    IL_1ed4:  ldloc.s    V_6
    IL_1ed6:  stelem.i4
    IL_1ed7:  ldloc.s    V_3
    IL_1ed9:  ldloc.s    V_2
    IL_1edb:  ldc.i4.1
    IL_1edc:  sub
    IL_1edd:  dup
    IL_1ede:  stloc.s    V_2
    IL_1ee0:  ldc.i4.s   11
    IL_1ee2:  stelem.i4
    IL_1ee3:  br         IL_02fa

    IL_1ee8:  ldloc.s    V_5
    IL_1eea:  ldloc.s    V_4
    IL_1eec:  ldloc.s    V_3
    IL_1eee:  ldloc.s    V_2
    IL_1ef0:  dup
    IL_1ef1:  ldc.i4.1
    IL_1ef2:  add
    IL_1ef3:  stloc.s    V_2
    IL_1ef5:  ldelem.i4
    IL_1ef6:  stelem.i4
    IL_1ef7:  ldloc.s    V_5
    IL_1ef9:  ldloc.s    V_4
    IL_1efb:  ldc.i4.1
    IL_1efc:  sub
    IL_1efd:  dup
    IL_1efe:  stloc.s    V_4
    IL_1f00:  ldloc.s    V_6
    IL_1f02:  stelem.i4
    IL_1f03:  br         IL_1cba

    IL_1f08:  ldloc.s    V_3
    IL_1f0a:  ldloc.s    V_2
    IL_1f0c:  dup
    IL_1f0d:  ldc.i4.1
    IL_1f0e:  add
    IL_1f0f:  stloc.s    V_2
    IL_1f11:  ldelem.i4
    IL_1f12:  stloc.s    V_6
    IL_1f14:  ldloc.s    V_5
    IL_1f16:  ldloc.s    V_4
    IL_1f18:  ldc.i4.1
    IL_1f19:  sub
    IL_1f1a:  dup
    IL_1f1b:  stloc.s    V_4
    IL_1f1d:  ldloc.s    V_3
    IL_1f1f:  ldloc.s    V_2
    IL_1f21:  dup
    IL_1f22:  ldc.i4.1
    IL_1f23:  add
    IL_1f24:  stloc.s    V_2
    IL_1f26:  ldelem.i4
    IL_1f27:  stelem.i4
    IL_1f28:  ldloc.s    V_5
    IL_1f2a:  ldloc.s    V_4
    IL_1f2c:  ldc.i4.1
    IL_1f2d:  sub
    IL_1f2e:  dup
    IL_1f2f:  stloc.s    V_4
    IL_1f31:  ldloc.s    V_6
    IL_1f33:  stelem.i4
    IL_1f34:  br         IL_1cba

    IL_1f39:  ldloc.s    V_3
    IL_1f3b:  ldloc.s    V_2
    IL_1f3d:  dup
    IL_1f3e:  ldc.i4.1
    IL_1f3f:  add
    IL_1f40:  stloc.s    V_2
    IL_1f42:  ldelem.i4
    IL_1f43:  stloc.s    V_0
    IL_1f45:  ldloc.s    V_3
    IL_1f47:  ldloc.s    V_2
    IL_1f49:  dup
    IL_1f4a:  ldc.i4.1
    IL_1f4b:  add
    IL_1f4c:  stloc.s    V_2
    IL_1f4e:  ldelem.i4
    IL_1f4f:  stloc.s    V_6
    IL_1f51:  ldloc.s    V_6
    IL_1f53:  ldc.i4.s   0
    IL_1f55:  ble        IL_03c7

    IL_1f5a:  ldloc.s    V_3
    IL_1f5c:  ldloc.s    V_2
    IL_1f5e:  ldc.i4.1
    IL_1f5f:  sub
    IL_1f60:  dup
    IL_1f61:  stloc.s    V_2
    IL_1f63:  ldloc.s    V_6
    IL_1f65:  ldc.i4.s   1
    IL_1f67:  sub
    IL_1f68:  stelem.i4
    IL_1f69:  ldloc.s    V_3
    IL_1f6b:  ldloc.s    V_2
    IL_1f6d:  ldc.i4.1
    IL_1f6e:  sub
    IL_1f6f:  dup
    IL_1f70:  stloc.s    V_2
    IL_1f72:  ldloc.s    V_0
    IL_1f74:  ldc.i4.s   1
    IL_1f76:  sub
    IL_1f77:  stelem.i4
    IL_1f78:  ldloc.s    V_3
    IL_1f7a:  ldloc.s    V_2
    IL_1f7c:  ldc.i4.1
    IL_1f7d:  sub
    IL_1f7e:  dup
    IL_1f7f:  stloc.s    V_2
    IL_1f81:  ldc.i4.s   12
    IL_1f83:  stelem.i4
    IL_1f84:  br         IL_03c7

    IL_1f89:  ldloc.s    V_3
    IL_1f8b:  ldloc.s    V_2
    IL_1f8d:  dup
    IL_1f8e:  ldc.i4.1
    IL_1f8f:  add
    IL_1f90:  stloc.s    V_2
    IL_1f92:  ldelem.i4
    IL_1f93:  stloc.s    V_0
    IL_1f95:  ldloc.s    V_3
    IL_1f97:  ldloc.s    V_2
    IL_1f99:  dup
    IL_1f9a:  ldc.i4.1
    IL_1f9b:  add
    IL_1f9c:  stloc.s    V_2
    IL_1f9e:  ldelem.i4
    IL_1f9f:  stloc.s    V_6
    IL_1fa1:  ldloc.s    V_6
    IL_1fa3:  ldc.i4.s   0
    IL_1fa5:  ble        IL_048f

    IL_1faa:  ldloc.s    V_3
    IL_1fac:  ldloc.s    V_2
    IL_1fae:  ldc.i4.1
    IL_1faf:  sub
    IL_1fb0:  dup
    IL_1fb1:  stloc.s    V_2
    IL_1fb3:  ldloc.s    V_6
    IL_1fb5:  ldc.i4.s   1
    IL_1fb7:  sub
    IL_1fb8:  stelem.i4
    IL_1fb9:  ldloc.s    V_3
    IL_1fbb:  ldloc.s    V_2
    IL_1fbd:  ldc.i4.1
    IL_1fbe:  sub
    IL_1fbf:  dup
    IL_1fc0:  stloc.s    V_2
    IL_1fc2:  ldloc.s    V_0
    IL_1fc4:  ldc.i4.s   1
    IL_1fc6:  sub
    IL_1fc7:  stelem.i4
    IL_1fc8:  ldloc.s    V_3
    IL_1fca:  ldloc.s    V_2
    IL_1fcc:  ldc.i4.1
    IL_1fcd:  sub
    IL_1fce:  dup
    IL_1fcf:  stloc.s    V_2
    IL_1fd1:  ldc.i4.s   13
    IL_1fd3:  stelem.i4
    IL_1fd4:  br         IL_048f

    IL_1fd9:  ldloc.s    V_3
    IL_1fdb:  ldloc.s    V_2
    IL_1fdd:  dup
    IL_1fde:  ldc.i4.1
    IL_1fdf:  add
    IL_1fe0:  stloc.s    V_2
    IL_1fe2:  ldelem.i4
    IL_1fe3:  stloc.s    V_0
    IL_1fe5:  ldloc.s    V_3
    IL_1fe7:  ldloc.s    V_2
    IL_1fe9:  dup
    IL_1fea:  ldc.i4.1
    IL_1feb:  add
    IL_1fec:  stloc.s    V_2
    IL_1fee:  ldelem.i4
    IL_1fef:  stloc.s    V_6
    IL_1ff1:  ldloc.s    V_6
    IL_1ff3:  ldc.i4.s   0
    IL_1ff5:  ble        IL_055c

    IL_1ffa:  ldloc.s    V_3
    IL_1ffc:  ldloc.s    V_2
    IL_1ffe:  ldc.i4.1
    IL_1fff:  sub
    IL_2000:  dup
    IL_2001:  stloc.s    V_2
    IL_2003:  ldloc.s    V_6
    IL_2005:  ldc.i4.s   1
    IL_2007:  sub
    IL_2008:  stelem.i4
    IL_2009:  ldloc.s    V_3
    IL_200b:  ldloc.s    V_2
    IL_200d:  ldc.i4.1
    IL_200e:  sub
    IL_200f:  dup
    IL_2010:  stloc.s    V_2
    IL_2012:  ldloc.s    V_0
    IL_2014:  ldc.i4.s   1
    IL_2016:  sub
    IL_2017:  stelem.i4
    IL_2018:  ldloc.s    V_3
    IL_201a:  ldloc.s    V_2
    IL_201c:  ldc.i4.1
    IL_201d:  sub
    IL_201e:  dup
    IL_201f:  stloc.s    V_2
    IL_2021:  ldc.i4.s   14
    IL_2023:  stelem.i4
    IL_2024:  br         IL_055c

    IL_2029:  ldloc.s    V_3
    IL_202b:  ldloc.s    V_2
    IL_202d:  dup
    IL_202e:  ldc.i4.1
    IL_202f:  add
    IL_2030:  stloc.s    V_2
    IL_2032:  ldelem.i4
    IL_2033:  stloc.s    V_0
    IL_2035:  ldloc.s    V_3
    IL_2037:  ldloc.s    V_2
    IL_2039:  dup
    IL_203a:  ldc.i4.1
    IL_203b:  add
    IL_203c:  stloc.s    V_2
    IL_203e:  ldelem.i4
    IL_203f:  stloc.s    V_6
    IL_2041:  ldloc.s    V_6
    IL_2043:  ldc.i4.s   0
    IL_2045:  ble        IL_0660

    IL_204a:  ldloc.s    V_3
    IL_204c:  ldloc.s    V_2
    IL_204e:  ldc.i4.1
    IL_204f:  sub
    IL_2050:  dup
    IL_2051:  stloc.s    V_2
    IL_2053:  ldloc.s    V_6
    IL_2055:  ldc.i4.s   1
    IL_2057:  sub
    IL_2058:  stelem.i4
    IL_2059:  ldloc.s    V_3
    IL_205b:  ldloc.s    V_2
    IL_205d:  ldc.i4.1
    IL_205e:  sub
    IL_205f:  dup
    IL_2060:  stloc.s    V_2
    IL_2062:  ldloc.s    V_0
    IL_2064:  ldc.i4.s   1
    IL_2066:  sub
    IL_2067:  stelem.i4
    IL_2068:  ldloc.s    V_3
    IL_206a:  ldloc.s    V_2
    IL_206c:  ldc.i4.1
    IL_206d:  sub
    IL_206e:  dup
    IL_206f:  stloc.s    V_2
    IL_2071:  ldc.i4.s   15
    IL_2073:  stelem.i4
    IL_2074:  br         IL_0660

    IL_2079:  ldloc.s    V_3
    IL_207b:  ldloc.s    V_2
    IL_207d:  dup
    IL_207e:  ldc.i4.1
    IL_207f:  add
    IL_2080:  stloc.s    V_2
    IL_2082:  ldelem.i4
    IL_2083:  stloc.s    V_0
    IL_2085:  br         IL_07bc

    IL_208a:  ldloc.s    V_3
    IL_208c:  ldloc.s    V_2
    IL_208e:  dup
    IL_208f:  ldc.i4.1
    IL_2090:  add
    IL_2091:  stloc.s    V_2
    IL_2093:  ldelem.i4
    IL_2094:  stloc.s    V_0
    IL_2096:  ldloc.s    V_3
    IL_2098:  ldloc.s    V_2
    IL_209a:  dup
    IL_209b:  ldc.i4.1
    IL_209c:  add
    IL_209d:  stloc.s    V_2
    IL_209f:  ldelem.i4
    IL_20a0:  stloc.s    V_6
    IL_20a2:  ldloc.s    V_6
    IL_20a4:  ldc.i4.s   0
    IL_20a6:  ble        IL_0781

    IL_20ab:  ldloc.s    V_3
    IL_20ad:  ldloc.s    V_2
    IL_20af:  ldc.i4.1
    IL_20b0:  sub
    IL_20b1:  dup
    IL_20b2:  stloc.s    V_2
    IL_20b4:  ldloc.s    V_6
    IL_20b6:  ldc.i4.s   1
    IL_20b8:  sub
    IL_20b9:  stelem.i4
    IL_20ba:  ldloc.s    V_3
    IL_20bc:  ldloc.s    V_2
    IL_20be:  ldc.i4.1
    IL_20bf:  sub
    IL_20c0:  dup
    IL_20c1:  stloc.s    V_2
    IL_20c3:  ldloc.s    V_0
    IL_20c5:  ldc.i4.s   1
    IL_20c7:  sub
    IL_20c8:  stelem.i4
    IL_20c9:  ldloc.s    V_3
    IL_20cb:  ldloc.s    V_2
    IL_20cd:  ldc.i4.1
    IL_20ce:  sub
    IL_20cf:  dup
    IL_20d0:  stloc.s    V_2
    IL_20d2:  ldc.i4.s   17
    IL_20d4:  stelem.i4
    IL_20d5:  br         IL_0781

    IL_20da:  ldloc.s    V_3
    IL_20dc:  ldloc.s    V_2
    IL_20de:  dup
    IL_20df:  ldc.i4.1
    IL_20e0:  add
    IL_20e1:  stloc.s    V_2
    IL_20e3:  ldelem.i4
    IL_20e4:  stloc.s    V_0
    IL_20e6:  ldloc.s    V_3
    IL_20e8:  ldloc.s    V_2
    IL_20ea:  dup
    IL_20eb:  ldc.i4.1
    IL_20ec:  add
    IL_20ed:  stloc.s    V_2
    IL_20ef:  ldelem.i4
    IL_20f0:  stloc.s    V_6
    IL_20f2:  ldloc.s    V_6
    IL_20f4:  ldc.i4.s   0
    IL_20f6:  ble        IL_08f8

    IL_20fb:  ldloc.s    V_3
    IL_20fd:  ldloc.s    V_2
    IL_20ff:  ldc.i4.1
    IL_2100:  sub
    IL_2101:  dup
    IL_2102:  stloc.s    V_2
    IL_2104:  ldloc.s    V_6
    IL_2106:  ldc.i4.s   1
    IL_2108:  sub
    IL_2109:  stelem.i4
    IL_210a:  ldloc.s    V_3
    IL_210c:  ldloc.s    V_2
    IL_210e:  ldc.i4.1
    IL_210f:  sub
    IL_2110:  dup
    IL_2111:  stloc.s    V_2
    IL_2113:  ldloc.s    V_0
    IL_2115:  ldc.i4.s   1
    IL_2117:  sub
    IL_2118:  stelem.i4
    IL_2119:  ldloc.s    V_3
    IL_211b:  ldloc.s    V_2
    IL_211d:  ldc.i4.1
    IL_211e:  sub
    IL_211f:  dup
    IL_2120:  stloc.s    V_2
    IL_2122:  ldc.i4.s   18
    IL_2124:  stelem.i4
    IL_2125:  br         IL_08f8

    IL_212a:  ldloc.s    V_5
    IL_212c:  ldloc.s    V_4
    IL_212e:  dup
    IL_212f:  ldc.i4.1
    IL_2130:  add
    IL_2131:  stloc.s    V_4
    IL_2133:  ldelem.i4
    IL_2134:  ldc.i4.s   1
    IL_2136:  sub
    IL_2137:  dup
    IL_2138:  stloc.s    V_6
    IL_213a:  ldc.i4.s   0
    IL_213c:  blt.s      IL_2167

    IL_213e:  ldloc.s    V_5
    IL_2140:  ldloc.s    V_4
    IL_2142:  dup
    IL_2143:  ldc.i4.1
    IL_2144:  add
    IL_2145:  stloc.s    V_4
    IL_2147:  ldelem.i4
    IL_2148:  stloc.s    V_0
    IL_214a:  ldloc.s    V_3
    IL_214c:  ldloc.s    V_2
    IL_214e:  ldc.i4.1
    IL_214f:  sub
    IL_2150:  dup
    IL_2151:  stloc.s    V_2
    IL_2153:  ldloc.s    V_6
    IL_2155:  stelem.i4
    IL_2156:  ldloc.s    V_3
    IL_2158:  ldloc.s    V_2
    IL_215a:  ldc.i4.1
    IL_215b:  sub
    IL_215c:  dup
    IL_215d:  stloc.s    V_2
    IL_215f:  ldc.i4.s   11
    IL_2161:  stelem.i4
    IL_2162:  br         IL_0a22

    IL_2167:  ldloc.s    V_5
    IL_2169:  ldloc.s    V_4
    IL_216b:  ldloc.s    V_3
    IL_216d:  ldloc.s    V_2
    IL_216f:  dup
    IL_2170:  ldc.i4.1
    IL_2171:  add
    IL_2172:  stloc.s    V_2
    IL_2174:  ldelem.i4
    IL_2175:  stelem.i4
    IL_2176:  ldloc.s    V_5
    IL_2178:  ldloc.s    V_4
    IL_217a:  ldc.i4.1
    IL_217b:  sub
    IL_217c:  dup
    IL_217d:  stloc.s    V_4
    IL_217f:  ldloc.s    V_6
    IL_2181:  stelem.i4
    IL_2182:  br         IL_1cba

    IL_2187:  ldloc.s    V_5
    IL_2189:  ldloc.s    V_4
    IL_218b:  dup
    IL_218c:  ldc.i4.1
    IL_218d:  add
    IL_218e:  stloc.s    V_4
    IL_2190:  ldelem.i4
    IL_2191:  ldc.i4.s   1
    IL_2193:  sub
    IL_2194:  dup
    IL_2195:  stloc.s    V_6
    IL_2197:  ldc.i4.s   0
    IL_2199:  blt.s      IL_21c4

    IL_219b:  ldloc.s    V_5
    IL_219d:  ldloc.s    V_4
    IL_219f:  dup
    IL_21a0:  ldc.i4.1
    IL_21a1:  add
    IL_21a2:  stloc.s    V_4
    IL_21a4:  ldelem.i4
    IL_21a5:  stloc.s    V_0
    IL_21a7:  ldloc.s    V_3
    IL_21a9:  ldloc.s    V_2
    IL_21ab:  ldc.i4.1
    IL_21ac:  sub
    IL_21ad:  dup
    IL_21ae:  stloc.s    V_2
    IL_21b0:  ldloc.s    V_6
    IL_21b2:  stelem.i4
    IL_21b3:  ldloc.s    V_3
    IL_21b5:  ldloc.s    V_2
    IL_21b7:  ldc.i4.1
    IL_21b8:  sub
    IL_21b9:  dup
    IL_21ba:  stloc.s    V_2
    IL_21bc:  ldc.i4.s   11
    IL_21be:  stelem.i4
    IL_21bf:  br         IL_0aec

    IL_21c4:  ldloc.s    V_5
    IL_21c6:  ldloc.s    V_4
    IL_21c8:  ldloc.s    V_3
    IL_21ca:  ldloc.s    V_2
    IL_21cc:  dup
    IL_21cd:  ldc.i4.1
    IL_21ce:  add
    IL_21cf:  stloc.s    V_2
    IL_21d1:  ldelem.i4
    IL_21d2:  stelem.i4
    IL_21d3:  ldloc.s    V_5
    IL_21d5:  ldloc.s    V_4
    IL_21d7:  ldc.i4.1
    IL_21d8:  sub
    IL_21d9:  dup
    IL_21da:  stloc.s    V_4
    IL_21dc:  ldloc.s    V_6
    IL_21de:  stelem.i4
    IL_21df:  br         IL_1cba

    IL_21e4:  ldloc.s    V_3
    IL_21e6:  ldloc.s    V_2
    IL_21e8:  dup
    IL_21e9:  ldc.i4.1
    IL_21ea:  add
    IL_21eb:  stloc.s    V_2
    IL_21ed:  ldelem.i4
    IL_21ee:  stloc.s    V_0
    IL_21f0:  ldloc.s    V_3
    IL_21f2:  ldloc.s    V_2
    IL_21f4:  dup
    IL_21f5:  ldc.i4.1
    IL_21f6:  add
    IL_21f7:  stloc.s    V_2
    IL_21f9:  ldelem.i4
    IL_21fa:  stloc.s    V_6
    IL_21fc:  ldloc.s    V_6
    IL_21fe:  ldc.i4.s   0
    IL_2200:  ble        IL_0c01

    IL_2205:  ldloc.s    V_3
    IL_2207:  ldloc.s    V_2
    IL_2209:  ldc.i4.1
    IL_220a:  sub
    IL_220b:  dup
    IL_220c:  stloc.s    V_2
    IL_220e:  ldloc.s    V_6
    IL_2210:  ldc.i4.s   1
    IL_2212:  sub
    IL_2213:  stelem.i4
    IL_2214:  ldloc.s    V_3
    IL_2216:  ldloc.s    V_2
    IL_2218:  ldc.i4.1
    IL_2219:  sub
    IL_221a:  dup
    IL_221b:  stloc.s    V_2
    IL_221d:  ldloc.s    V_0
    IL_221f:  ldc.i4.s   1
    IL_2221:  sub
    IL_2222:  stelem.i4
    IL_2223:  ldloc.s    V_3
    IL_2225:  ldloc.s    V_2
    IL_2227:  ldc.i4.1
    IL_2228:  sub
    IL_2229:  dup
    IL_222a:  stloc.s    V_2
    IL_222c:  ldc.i4.s   23
    IL_222e:  stelem.i4
    IL_222f:  br         IL_0c01

    IL_2234:  ldloc.s    V_5
    IL_2236:  ldloc.s    V_4
    IL_2238:  dup
    IL_2239:  ldc.i4.1
    IL_223a:  add
    IL_223b:  stloc.s    V_4
    IL_223d:  ldelem.i4
    IL_223e:  ldc.i4.s   1
    IL_2240:  sub
    IL_2241:  dup
    IL_2242:  stloc.s    V_6
    IL_2244:  ldc.i4.s   0
    IL_2246:  blt.s      IL_2271

    IL_2248:  ldloc.s    V_5
    IL_224a:  ldloc.s    V_4
    IL_224c:  dup
    IL_224d:  ldc.i4.1
    IL_224e:  add
    IL_224f:  stloc.s    V_4
    IL_2251:  ldelem.i4
    IL_2252:  stloc.s    V_0
    IL_2254:  ldloc.s    V_3
    IL_2256:  ldloc.s    V_2
    IL_2258:  ldc.i4.1
    IL_2259:  sub
    IL_225a:  dup
    IL_225b:  stloc.s    V_2
    IL_225d:  ldloc.s    V_6
    IL_225f:  stelem.i4
    IL_2260:  ldloc.s    V_3
    IL_2262:  ldloc.s    V_2
    IL_2264:  ldc.i4.1
    IL_2265:  sub
    IL_2266:  dup
    IL_2267:  stloc.s    V_2
    IL_2269:  ldc.i4.s   11
    IL_226b:  stelem.i4
    IL_226c:  br         IL_0cea

    IL_2271:  ldloc.s    V_5
    IL_2273:  ldloc.s    V_4
    IL_2275:  ldloc.s    V_3
    IL_2277:  ldloc.s    V_2
    IL_2279:  dup
    IL_227a:  ldc.i4.1
    IL_227b:  add
    IL_227c:  stloc.s    V_2
    IL_227e:  ldelem.i4
    IL_227f:  stelem.i4
    IL_2280:  ldloc.s    V_5
    IL_2282:  ldloc.s    V_4
    IL_2284:  ldc.i4.1
    IL_2285:  sub
    IL_2286:  dup
    IL_2287:  stloc.s    V_4
    IL_2289:  ldloc.s    V_6
    IL_228b:  stelem.i4
    IL_228c:  br         IL_1cba

    IL_2291:  ldloc.s    V_3
    IL_2293:  ldloc.s    V_2
    IL_2295:  dup
    IL_2296:  ldc.i4.1
    IL_2297:  add
    IL_2298:  stloc.s    V_2
    IL_229a:  ldelem.i4
    IL_229b:  stloc.s    V_0
    IL_229d:  br         IL_0e12

    IL_22a2:  ldloc.s    V_3
    IL_22a4:  ldloc.s    V_2
    IL_22a6:  dup
    IL_22a7:  ldc.i4.1
    IL_22a8:  add
    IL_22a9:  stloc.s    V_2
    IL_22ab:  ldelem.i4
    IL_22ac:  stloc.s    V_0
    IL_22ae:  ldloc.s    V_3
    IL_22b0:  ldloc.s    V_2
    IL_22b2:  dup
    IL_22b3:  ldc.i4.1
    IL_22b4:  add
    IL_22b5:  stloc.s    V_2
    IL_22b7:  ldelem.i4
    IL_22b8:  stloc.s    V_6
    IL_22ba:  ldloc.s    V_6
    IL_22bc:  ldc.i4.s   0
    IL_22be:  ble        IL_0dee

    IL_22c3:  ldloc.s    V_3
    IL_22c5:  ldloc.s    V_2
    IL_22c7:  ldc.i4.1
    IL_22c8:  sub
    IL_22c9:  dup
    IL_22ca:  stloc.s    V_2
    IL_22cc:  ldloc.s    V_6
    IL_22ce:  ldc.i4.s   1
    IL_22d0:  sub
    IL_22d1:  stelem.i4
    IL_22d2:  ldloc.s    V_3
    IL_22d4:  ldloc.s    V_2
    IL_22d6:  ldc.i4.1
    IL_22d7:  sub
    IL_22d8:  dup
    IL_22d9:  stloc.s    V_2
    IL_22db:  ldloc.s    V_0
    IL_22dd:  ldc.i4.s   1
    IL_22df:  sub
    IL_22e0:  stelem.i4
    IL_22e1:  ldloc.s    V_3
    IL_22e3:  ldloc.s    V_2
    IL_22e5:  ldc.i4.1
    IL_22e6:  sub
    IL_22e7:  dup
    IL_22e8:  stloc.s    V_2
    IL_22ea:  ldc.i4.s   27
    IL_22ec:  stelem.i4
    IL_22ed:  br         IL_0dee

    IL_22f2:  ldloc.s    V_3
    IL_22f4:  ldloc.s    V_2
    IL_22f6:  dup
    IL_22f7:  ldc.i4.1
    IL_22f8:  add
    IL_22f9:  stloc.s    V_2
    IL_22fb:  ldelem.i4
    IL_22fc:  stloc.s    V_0
    IL_22fe:  br         IL_0f1a

    IL_2303:  ldloc.s    V_3
    IL_2305:  ldloc.s    V_2
    IL_2307:  dup
    IL_2308:  ldc.i4.1
    IL_2309:  add
    IL_230a:  stloc.s    V_2
    IL_230c:  ldelem.i4
    IL_230d:  stloc.s    V_0
    IL_230f:  ldloc.s    V_3
    IL_2311:  ldloc.s    V_2
    IL_2313:  dup
    IL_2314:  ldc.i4.1
    IL_2315:  add
    IL_2316:  stloc.s    V_2
    IL_2318:  ldelem.i4
    IL_2319:  stloc.s    V_6
    IL_231b:  ldloc.s    V_6
    IL_231d:  ldc.i4.s   0
    IL_231f:  ble        IL_0edf

    IL_2324:  ldloc.s    V_3
    IL_2326:  ldloc.s    V_2
    IL_2328:  ldc.i4.1
    IL_2329:  sub
    IL_232a:  dup
    IL_232b:  stloc.s    V_2
    IL_232d:  ldloc.s    V_6
    IL_232f:  ldc.i4.s   1
    IL_2331:  sub
    IL_2332:  stelem.i4
    IL_2333:  ldloc.s    V_3
    IL_2335:  ldloc.s    V_2
    IL_2337:  ldc.i4.1
    IL_2338:  sub
    IL_2339:  dup
    IL_233a:  stloc.s    V_2
    IL_233c:  ldloc.s    V_0
    IL_233e:  ldc.i4.s   1
    IL_2340:  sub
    IL_2341:  stelem.i4
    IL_2342:  ldloc.s    V_3
    IL_2344:  ldloc.s    V_2
    IL_2346:  ldc.i4.1
    IL_2347:  sub
    IL_2348:  dup
    IL_2349:  stloc.s    V_2
    IL_234b:  ldc.i4.s   29
    IL_234d:  stelem.i4
    IL_234e:  br         IL_0edf

    IL_2353:  ldloc.s    V_3
    IL_2355:  ldloc.s    V_2
    IL_2357:  dup
    IL_2358:  ldc.i4.1
    IL_2359:  add
    IL_235a:  stloc.s    V_2
    IL_235c:  ldelem.i4
    IL_235d:  stloc.s    V_0
    IL_235f:  ldloc.s    V_3
    IL_2361:  ldloc.s    V_2
    IL_2363:  dup
    IL_2364:  ldc.i4.1
    IL_2365:  add
    IL_2366:  stloc.s    V_2
    IL_2368:  ldelem.i4
    IL_2369:  stloc.s    V_6
    IL_236b:  ldloc.s    V_6
    IL_236d:  ldc.i4.s   0
    IL_236f:  ble        IL_0fcf

    IL_2374:  ldloc.s    V_3
    IL_2376:  ldloc.s    V_2
    IL_2378:  ldc.i4.1
    IL_2379:  sub
    IL_237a:  dup
    IL_237b:  stloc.s    V_2
    IL_237d:  ldloc.s    V_6
    IL_237f:  ldc.i4.s   1
    IL_2381:  sub
    IL_2382:  stelem.i4
    IL_2383:  ldloc.s    V_3
    IL_2385:  ldloc.s    V_2
    IL_2387:  ldc.i4.1
    IL_2388:  sub
    IL_2389:  dup
    IL_238a:  stloc.s    V_2
    IL_238c:  ldloc.s    V_0
    IL_238e:  ldc.i4.s   1
    IL_2390:  sub
    IL_2391:  stelem.i4
    IL_2392:  ldloc.s    V_3
    IL_2394:  ldloc.s    V_2
    IL_2396:  ldc.i4.1
    IL_2397:  sub
    IL_2398:  dup
    IL_2399:  stloc.s    V_2
    IL_239b:  ldc.i4.s   30
    IL_239d:  stelem.i4
    IL_239e:  br         IL_0fcf

    IL_23a3:  ldloc.s    V_3
    IL_23a5:  ldloc.s    V_2
    IL_23a7:  dup
    IL_23a8:  ldc.i4.1
    IL_23a9:  add
    IL_23aa:  stloc.s    V_2
    IL_23ac:  ldelem.i4
    IL_23ad:  stloc.s    V_0
    IL_23af:  ldloc.s    V_3
    IL_23b1:  ldloc.s    V_2
    IL_23b3:  dup
    IL_23b4:  ldc.i4.1
    IL_23b5:  add
    IL_23b6:  stloc.s    V_2
    IL_23b8:  ldelem.i4
    IL_23b9:  stloc.s    V_6
    IL_23bb:  ldloc.s    V_6
    IL_23bd:  ldc.i4.s   0
    IL_23bf:  ble        IL_10a3

    IL_23c4:  ldloc.s    V_3
    IL_23c6:  ldloc.s    V_2
    IL_23c8:  ldc.i4.1
    IL_23c9:  sub
    IL_23ca:  dup
    IL_23cb:  stloc.s    V_2
    IL_23cd:  ldloc.s    V_6
    IL_23cf:  ldc.i4.s   1
    IL_23d1:  sub
    IL_23d2:  stelem.i4
    IL_23d3:  ldloc.s    V_3
    IL_23d5:  ldloc.s    V_2
    IL_23d7:  ldc.i4.1
    IL_23d8:  sub
    IL_23d9:  dup
    IL_23da:  stloc.s    V_2
    IL_23dc:  ldloc.s    V_0
    IL_23de:  ldc.i4.s   1
    IL_23e0:  sub
    IL_23e1:  stelem.i4
    IL_23e2:  ldloc.s    V_3
    IL_23e4:  ldloc.s    V_2
    IL_23e6:  ldc.i4.1
    IL_23e7:  sub
    IL_23e8:  dup
    IL_23e9:  stloc.s    V_2
    IL_23eb:  ldc.i4.s   31
    IL_23ed:  stelem.i4
    IL_23ee:  br         IL_10a3

    IL_23f3:  ldloc.s    V_5
    IL_23f5:  ldloc.s    V_4
    IL_23f7:  dup
    IL_23f8:  ldc.i4.1
    IL_23f9:  add
    IL_23fa:  stloc.s    V_4
    IL_23fc:  ldelem.i4
    IL_23fd:  ldc.i4.s   1
    IL_23ff:  sub
    IL_2400:  dup
    IL_2401:  stloc.s    V_6
    IL_2403:  ldc.i4.s   0
    IL_2405:  blt.s      IL_2430

    IL_2407:  ldloc.s    V_5
    IL_2409:  ldloc.s    V_4
    IL_240b:  dup
    IL_240c:  ldc.i4.1
    IL_240d:  add
    IL_240e:  stloc.s    V_4
    IL_2410:  ldelem.i4
    IL_2411:  stloc.s    V_0
    IL_2413:  ldloc.s    V_3
    IL_2415:  ldloc.s    V_2
    IL_2417:  ldc.i4.1
    IL_2418:  sub
    IL_2419:  dup
    IL_241a:  stloc.s    V_2
    IL_241c:  ldloc.s    V_6
    IL_241e:  stelem.i4
    IL_241f:  ldloc.s    V_3
    IL_2421:  ldloc.s    V_2
    IL_2423:  ldc.i4.1
    IL_2424:  sub
    IL_2425:  dup
    IL_2426:  stloc.s    V_2
    IL_2428:  ldc.i4.s   11
    IL_242a:  stelem.i4
    IL_242b:  br         IL_1262

    IL_2430:  ldloc.s    V_5
    IL_2432:  ldloc.s    V_4
    IL_2434:  ldloc.s    V_3
    IL_2436:  ldloc.s    V_2
    IL_2438:  dup
    IL_2439:  ldc.i4.1
    IL_243a:  add
    IL_243b:  stloc.s    V_2
    IL_243d:  ldelem.i4
    IL_243e:  stelem.i4
    IL_243f:  ldloc.s    V_5
    IL_2441:  ldloc.s    V_4
    IL_2443:  ldc.i4.1
    IL_2444:  sub
    IL_2445:  dup
    IL_2446:  stloc.s    V_4
    IL_2448:  ldloc.s    V_6
    IL_244a:  stelem.i4
    IL_244b:  br         IL_1cba

    IL_2450:  ldloc.s    V_3
    IL_2452:  ldloc.s    V_2
    IL_2454:  dup
    IL_2455:  ldc.i4.1
    IL_2456:  add
    IL_2457:  stloc.s    V_2
    IL_2459:  ldelem.i4
    IL_245a:  stloc.s    V_0
    IL_245c:  br         IL_1409

    IL_2461:  ldloc.s    V_3
    IL_2463:  ldloc.s    V_2
    IL_2465:  dup
    IL_2466:  ldc.i4.1
    IL_2467:  add
    IL_2468:  stloc.s    V_2
    IL_246a:  ldelem.i4
    IL_246b:  stloc.s    V_0
    IL_246d:  ldloc.s    V_3
    IL_246f:  ldloc.s    V_2
    IL_2471:  dup
    IL_2472:  ldc.i4.1
    IL_2473:  add
    IL_2474:  stloc.s    V_2
    IL_2476:  ldelem.i4
    IL_2477:  stloc.s    V_6
    IL_2479:  ldloc.s    V_6
    IL_247b:  ldc.i4.s   0
    IL_247d:  ble        IL_132f

    IL_2482:  ldloc.s    V_3
    IL_2484:  ldloc.s    V_2
    IL_2486:  ldc.i4.1
    IL_2487:  sub
    IL_2488:  dup
    IL_2489:  stloc.s    V_2
    IL_248b:  ldloc.s    V_6
    IL_248d:  ldc.i4.s   1
    IL_248f:  sub
    IL_2490:  stelem.i4
    IL_2491:  ldloc.s    V_3
    IL_2493:  ldloc.s    V_2
    IL_2495:  ldc.i4.1
    IL_2496:  sub
    IL_2497:  dup
    IL_2498:  stloc.s    V_2
    IL_249a:  ldloc.s    V_0
    IL_249c:  ldc.i4.s   1
    IL_249e:  sub
    IL_249f:  stelem.i4
    IL_24a0:  ldloc.s    V_3
    IL_24a2:  ldloc.s    V_2
    IL_24a4:  ldc.i4.1
    IL_24a5:  sub
    IL_24a6:  dup
    IL_24a7:  stloc.s    V_2
    IL_24a9:  ldc.i4.s   35
    IL_24ab:  stelem.i4
    IL_24ac:  br         IL_132f

    IL_24b1:  ldloc.s    V_3
    IL_24b3:  ldloc.s    V_2
    IL_24b5:  dup
    IL_24b6:  ldc.i4.1
    IL_24b7:  add
    IL_24b8:  stloc.s    V_2
    IL_24ba:  ldelem.i4
    IL_24bb:  stloc.s    V_0
    IL_24bd:  ldloc.s    V_3
    IL_24bf:  ldloc.s    V_2
    IL_24c1:  dup
    IL_24c2:  ldc.i4.1
    IL_24c3:  add
    IL_24c4:  stloc.s    V_2
    IL_24c6:  ldelem.i4
    IL_24c7:  stloc.s    V_6
    IL_24c9:  ldloc.s    V_6
    IL_24cb:  ldc.i4.s   0
    IL_24cd:  ble        IL_13d4

    IL_24d2:  ldloc.s    V_3
    IL_24d4:  ldloc.s    V_2
    IL_24d6:  ldc.i4.1
    IL_24d7:  sub
    IL_24d8:  dup
    IL_24d9:  stloc.s    V_2
    IL_24db:  ldloc.s    V_6
    IL_24dd:  ldc.i4.s   1
    IL_24df:  sub
    IL_24e0:  stelem.i4
    IL_24e1:  ldloc.s    V_3
    IL_24e3:  ldloc.s    V_2
    IL_24e5:  ldc.i4.1
    IL_24e6:  sub
    IL_24e7:  dup
    IL_24e8:  stloc.s    V_2
    IL_24ea:  ldloc.s    V_0
    IL_24ec:  ldc.i4.s   1
    IL_24ee:  sub
    IL_24ef:  stelem.i4
    IL_24f0:  ldloc.s    V_3
    IL_24f2:  ldloc.s    V_2
    IL_24f4:  ldc.i4.1
    IL_24f5:  sub
    IL_24f6:  dup
    IL_24f7:  stloc.s    V_2
    IL_24f9:  ldc.i4.s   36
    IL_24fb:  stelem.i4
    IL_24fc:  br         IL_13d4

    IL_2501:  ldloc.s    V_3
    IL_2503:  ldloc.s    V_2
    IL_2505:  dup
    IL_2506:  ldc.i4.1
    IL_2507:  add
    IL_2508:  stloc.s    V_2
    IL_250a:  ldelem.i4
    IL_250b:  stloc.s    V_0
    IL_250d:  br         IL_151a

    IL_2512:  ldloc.s    V_3
    IL_2514:  ldloc.s    V_2
    IL_2516:  dup
    IL_2517:  ldc.i4.1
    IL_2518:  add
    IL_2519:  stloc.s    V_2
    IL_251b:  ldelem.i4
    IL_251c:  stloc.s    V_0
    IL_251e:  ldloc.s    V_3
    IL_2520:  ldloc.s    V_2
    IL_2522:  dup
    IL_2523:  ldc.i4.1
    IL_2524:  add
    IL_2525:  stloc.s    V_2
    IL_2527:  ldelem.i4
    IL_2528:  stloc.s    V_6
    IL_252a:  ldloc.s    V_6
    IL_252c:  ldc.i4.s   0
    IL_252e:  ble        IL_14e5

    IL_2533:  ldloc.s    V_3
    IL_2535:  ldloc.s    V_2
    IL_2537:  ldc.i4.1
    IL_2538:  sub
    IL_2539:  dup
    IL_253a:  stloc.s    V_2
    IL_253c:  ldloc.s    V_6
    IL_253e:  ldc.i4.s   1
    IL_2540:  sub
    IL_2541:  stelem.i4
    IL_2542:  ldloc.s    V_3
    IL_2544:  ldloc.s    V_2
    IL_2546:  ldc.i4.1
    IL_2547:  sub
    IL_2548:  dup
    IL_2549:  stloc.s    V_2
    IL_254b:  ldloc.s    V_0
    IL_254d:  ldc.i4.s   1
    IL_254f:  sub
    IL_2550:  stelem.i4
    IL_2551:  ldloc.s    V_3
    IL_2553:  ldloc.s    V_2
    IL_2555:  ldc.i4.1
    IL_2556:  sub
    IL_2557:  dup
    IL_2558:  stloc.s    V_2
    IL_255a:  ldc.i4.s   38
    IL_255c:  stelem.i4
    IL_255d:  br         IL_14e5

    IL_2562:  ldloc.s    V_3
    IL_2564:  ldloc.s    V_2
    IL_2566:  dup
    IL_2567:  ldc.i4.1
    IL_2568:  add
    IL_2569:  stloc.s    V_2
    IL_256b:  ldelem.i4
    IL_256c:  stloc.s    V_0
    IL_256e:  ldloc.s    V_3
    IL_2570:  ldloc.s    V_2
    IL_2572:  dup
    IL_2573:  ldc.i4.1
    IL_2574:  add
    IL_2575:  stloc.s    V_2
    IL_2577:  ldelem.i4
    IL_2578:  stloc.s    V_6
    IL_257a:  ldloc.s    V_6
    IL_257c:  ldc.i4.s   0
    IL_257e:  ble        IL_15ad

    IL_2583:  ldloc.s    V_3
    IL_2585:  ldloc.s    V_2
    IL_2587:  ldc.i4.1
    IL_2588:  sub
    IL_2589:  dup
    IL_258a:  stloc.s    V_2
    IL_258c:  ldloc.s    V_6
    IL_258e:  ldc.i4.s   1
    IL_2590:  sub
    IL_2591:  stelem.i4
    IL_2592:  ldloc.s    V_3
    IL_2594:  ldloc.s    V_2
    IL_2596:  ldc.i4.1
    IL_2597:  sub
    IL_2598:  dup
    IL_2599:  stloc.s    V_2
    IL_259b:  ldloc.s    V_0
    IL_259d:  ldc.i4.s   1
    IL_259f:  sub
    IL_25a0:  stelem.i4
    IL_25a1:  ldloc.s    V_3
    IL_25a3:  ldloc.s    V_2
    IL_25a5:  ldc.i4.1
    IL_25a6:  sub
    IL_25a7:  dup
    IL_25a8:  stloc.s    V_2
    IL_25aa:  ldc.i4.s   39
    IL_25ac:  stelem.i4
    IL_25ad:  br         IL_15ad

    IL_25b2:  ldloc.s    V_3
    IL_25b4:  ldloc.s    V_2
    IL_25b6:  dup
    IL_25b7:  ldc.i4.1
    IL_25b8:  add
    IL_25b9:  stloc.s    V_2
    IL_25bb:  ldelem.i4
    IL_25bc:  stloc.s    V_0
    IL_25be:  ldloc.s    V_3
    IL_25c0:  ldloc.s    V_2
    IL_25c2:  dup
    IL_25c3:  ldc.i4.1
    IL_25c4:  add
    IL_25c5:  stloc.s    V_2
    IL_25c7:  ldelem.i4
    IL_25c8:  stloc.s    V_6
    IL_25ca:  ldloc.s    V_6
    IL_25cc:  ldc.i4.s   0
    IL_25ce:  ble        IL_167a

    IL_25d3:  ldloc.s    V_3
    IL_25d5:  ldloc.s    V_2
    IL_25d7:  ldc.i4.1
    IL_25d8:  sub
    IL_25d9:  dup
    IL_25da:  stloc.s    V_2
    IL_25dc:  ldloc.s    V_6
    IL_25de:  ldc.i4.s   1
    IL_25e0:  sub
    IL_25e1:  stelem.i4
    IL_25e2:  ldloc.s    V_3
    IL_25e4:  ldloc.s    V_2
    IL_25e6:  ldc.i4.1
    IL_25e7:  sub
    IL_25e8:  dup
    IL_25e9:  stloc.s    V_2
    IL_25eb:  ldloc.s    V_0
    IL_25ed:  ldc.i4.s   1
    IL_25ef:  sub
    IL_25f0:  stelem.i4
    IL_25f1:  ldloc.s    V_3
    IL_25f3:  ldloc.s    V_2
    IL_25f5:  ldc.i4.1
    IL_25f6:  sub
    IL_25f7:  dup
    IL_25f8:  stloc.s    V_2
    IL_25fa:  ldc.i4.s   40
    IL_25fc:  stelem.i4
    IL_25fd:  br         IL_167a

    IL_2602:  ldloc.s    V_3
    IL_2604:  ldloc.s    V_2
    IL_2606:  dup
    IL_2607:  ldc.i4.1
    IL_2608:  add
    IL_2609:  stloc.s    V_2
    IL_260b:  ldelem.i4
    IL_260c:  stloc.s    V_0
    IL_260e:  ldloc.s    V_3
    IL_2610:  ldloc.s    V_2
    IL_2612:  dup
    IL_2613:  ldc.i4.1
    IL_2614:  add
    IL_2615:  stloc.s    V_2
    IL_2617:  ldelem.i4
    IL_2618:  stloc.s    V_6
    IL_261a:  ldloc.s    V_6
    IL_261c:  ldc.i4.s   0
    IL_261e:  ble        IL_177e

    IL_2623:  ldloc.s    V_3
    IL_2625:  ldloc.s    V_2
    IL_2627:  ldc.i4.1
    IL_2628:  sub
    IL_2629:  dup
    IL_262a:  stloc.s    V_2
    IL_262c:  ldloc.s    V_6
    IL_262e:  ldc.i4.s   1
    IL_2630:  sub
    IL_2631:  stelem.i4
    IL_2632:  ldloc.s    V_3
    IL_2634:  ldloc.s    V_2
    IL_2636:  ldc.i4.1
    IL_2637:  sub
    IL_2638:  dup
    IL_2639:  stloc.s    V_2
    IL_263b:  ldloc.s    V_0
    IL_263d:  ldc.i4.s   1
    IL_263f:  sub
    IL_2640:  stelem.i4
    IL_2641:  ldloc.s    V_3
    IL_2643:  ldloc.s    V_2
    IL_2645:  ldc.i4.1
    IL_2646:  sub
    IL_2647:  dup
    IL_2648:  stloc.s    V_2
    IL_264a:  ldc.i4.s   41
    IL_264c:  stelem.i4
    IL_264d:  br         IL_177e

    IL_2652:  ldloc.s    V_5
    IL_2654:  ldloc.s    V_4
    IL_2656:  dup
    IL_2657:  ldc.i4.1
    IL_2658:  add
    IL_2659:  stloc.s    V_4
    IL_265b:  ldelem.i4
    IL_265c:  ldc.i4.s   1
    IL_265e:  sub
    IL_265f:  dup
    IL_2660:  stloc.s    V_6
    IL_2662:  ldc.i4.s   0
    IL_2664:  blt.s      IL_268f

    IL_2666:  ldloc.s    V_5
    IL_2668:  ldloc.s    V_4
    IL_266a:  dup
    IL_266b:  ldc.i4.1
    IL_266c:  add
    IL_266d:  stloc.s    V_4
    IL_266f:  ldelem.i4
    IL_2670:  stloc.s    V_0
    IL_2672:  ldloc.s    V_3
    IL_2674:  ldloc.s    V_2
    IL_2676:  ldc.i4.1
    IL_2677:  sub
    IL_2678:  dup
    IL_2679:  stloc.s    V_2
    IL_267b:  ldloc.s    V_6
    IL_267d:  stelem.i4
    IL_267e:  ldloc.s    V_3
    IL_2680:  ldloc.s    V_2
    IL_2682:  ldc.i4.1
    IL_2683:  sub
    IL_2684:  dup
    IL_2685:  stloc.s    V_2
    IL_2687:  ldc.i4.s   11
    IL_2689:  stelem.i4
    IL_268a:  br         IL_1848

    IL_268f:  ldloc.s    V_5
    IL_2691:  ldloc.s    V_4
    IL_2693:  ldloc.s    V_3
    IL_2695:  ldloc.s    V_2
    IL_2697:  dup
    IL_2698:  ldc.i4.1
    IL_2699:  add
    IL_269a:  stloc.s    V_2
    IL_269c:  ldelem.i4
    IL_269d:  stelem.i4
    IL_269e:  ldloc.s    V_5
    IL_26a0:  ldloc.s    V_4
    IL_26a2:  ldc.i4.1
    IL_26a3:  sub
    IL_26a4:  dup
    IL_26a5:  stloc.s    V_4
    IL_26a7:  ldloc.s    V_6
    IL_26a9:  stelem.i4
    IL_26aa:  br         IL_1cba

    IL_26af:  ldloc.s    V_3
    IL_26b1:  ldloc.s    V_2
    IL_26b3:  dup
    IL_26b4:  ldc.i4.1
    IL_26b5:  add
    IL_26b6:  stloc.s    V_2
    IL_26b8:  ldelem.i4
    IL_26b9:  stloc.s    V_0
    IL_26bb:  ldloc.s    V_5
    IL_26bd:  ldloc.s    V_4
    IL_26bf:  dup
    IL_26c0:  ldc.i4.1
    IL_26c1:  add
    IL_26c2:  stloc.s    V_4
    IL_26c4:  ldelem.i4
    IL_26c5:  pop
    IL_26c6:  ldloc.s    V_3
    IL_26c8:  ldloc.s    V_2
    IL_26ca:  ldc.i4.1
    IL_26cb:  sub
    IL_26cc:  dup
    IL_26cd:  stloc.s    V_2
    IL_26cf:  ldc.i4.s   46
    IL_26d1:  stelem.i4
    IL_26d2:  br         IL_18eb

    IL_26d7:  ldloc.s    V_5
    IL_26d9:  ldloc.s    V_4
    IL_26db:  ldc.i4.1
    IL_26dc:  sub
    IL_26dd:  dup
    IL_26de:  stloc.s    V_4
    IL_26e0:  ldloc.s    V_3
    IL_26e2:  ldloc.s    V_2
    IL_26e4:  dup
    IL_26e5:  ldc.i4.1
    IL_26e6:  add
    IL_26e7:  stloc.s    V_2
    IL_26e9:  ldelem.i4
    IL_26ea:  stelem.i4
    IL_26eb:  br         IL_1cba

    IL_26f0:  ldloc.s    V_5
    IL_26f2:  ldloc.s    V_4
    IL_26f4:  dup
    IL_26f5:  ldc.i4.1
    IL_26f6:  add
    IL_26f7:  stloc.s    V_4
    IL_26f9:  ldelem.i4
    IL_26fa:  ldc.i4.s   1
    IL_26fc:  sub
    IL_26fd:  dup
    IL_26fe:  stloc.s    V_6
    IL_2700:  ldc.i4.s   0
    IL_2702:  blt.s      IL_272d

    IL_2704:  ldloc.s    V_5
    IL_2706:  ldloc.s    V_4
    IL_2708:  dup
    IL_2709:  ldc.i4.1
    IL_270a:  add
    IL_270b:  stloc.s    V_4
    IL_270d:  ldelem.i4
    IL_270e:  stloc.s    V_0
    IL_2710:  ldloc.s    V_3
    IL_2712:  ldloc.s    V_2
    IL_2714:  ldc.i4.1
    IL_2715:  sub
    IL_2716:  dup
    IL_2717:  stloc.s    V_2
    IL_2719:  ldloc.s    V_6
    IL_271b:  stelem.i4
    IL_271c:  ldloc.s    V_3
    IL_271e:  ldloc.s    V_2
    IL_2720:  ldc.i4.1
    IL_2721:  sub
    IL_2722:  dup
    IL_2723:  stloc.s    V_2
    IL_2725:  ldc.i4.s   11
    IL_2727:  stelem.i4
    IL_2728:  br         IL_19b5

    IL_272d:  ldloc.s    V_5
    IL_272f:  ldloc.s    V_4
    IL_2731:  ldloc.s    V_3
    IL_2733:  ldloc.s    V_2
    IL_2735:  dup
    IL_2736:  ldc.i4.1
    IL_2737:  add
    IL_2738:  stloc.s    V_2
    IL_273a:  ldelem.i4
    IL_273b:  stelem.i4
    IL_273c:  ldloc.s    V_5
    IL_273e:  ldloc.s    V_4
    IL_2740:  ldc.i4.1
    IL_2741:  sub
    IL_2742:  dup
    IL_2743:  stloc.s    V_4
    IL_2745:  ldloc.s    V_6
    IL_2747:  stelem.i4
    IL_2748:  br         IL_1cba

    IL_274d:  ldloc.s    V_3
    IL_274f:  ldloc.s    V_2
    IL_2751:  dup
    IL_2752:  ldc.i4.1
    IL_2753:  add
    IL_2754:  stloc.s    V_2
    IL_2756:  ldelem.i4
    IL_2757:  stloc.s    V_0
    IL_2759:  br         IL_1b77

    IL_275e:  ldloc.s    V_3
    IL_2760:  ldloc.s    V_2
    IL_2762:  dup
    IL_2763:  ldc.i4.1
    IL_2764:  add
    IL_2765:  stloc.s    V_2
    IL_2767:  ldelem.i4
    IL_2768:  stloc.s    V_0
    IL_276a:  ldloc.s    V_3
    IL_276c:  ldloc.s    V_2
    IL_276e:  dup
    IL_276f:  ldc.i4.1
    IL_2770:  add
    IL_2771:  stloc.s    V_2
    IL_2773:  ldelem.i4
    IL_2774:  stloc.s    V_6
    IL_2776:  ldloc.s    V_6
    IL_2778:  ldc.i4.s   0
    IL_277a:  ble        IL_1b3c

    IL_277f:  ldloc.s    V_3
    IL_2781:  ldloc.s    V_2
    IL_2783:  ldc.i4.1
    IL_2784:  sub
    IL_2785:  dup
    IL_2786:  stloc.s    V_2
    IL_2788:  ldloc.s    V_6
    IL_278a:  ldc.i4.s   1
    IL_278c:  sub
    IL_278d:  stelem.i4
    IL_278e:  ldloc.s    V_3
    IL_2790:  ldloc.s    V_2
    IL_2792:  ldc.i4.1
    IL_2793:  sub
    IL_2794:  dup
    IL_2795:  stloc.s    V_2
    IL_2797:  ldloc.s    V_0
    IL_2799:  ldc.i4.s   1
    IL_279b:  sub
    IL_279c:  stelem.i4
    IL_279d:  ldloc.s    V_3
    IL_279f:  ldloc.s    V_2
    IL_27a1:  ldc.i4.1
    IL_27a2:  sub
    IL_27a3:  dup
    IL_27a4:  stloc.s    V_2
    IL_27a6:  ldc.i4.s   50
    IL_27a8:  stelem.i4
    IL_27a9:  br         IL_1b3c

    IL_27ae:  ldloc.s    V_3
    IL_27b0:  ldloc.s    V_2
    IL_27b2:  dup
    IL_27b3:  ldc.i4.1
    IL_27b4:  add
    IL_27b5:  stloc.s    V_2
    IL_27b7:  ldelem.i4
    IL_27b8:  stloc.s    V_0
    IL_27ba:  ldloc.s    V_3
    IL_27bc:  ldloc.s    V_2
    IL_27be:  dup
    IL_27bf:  ldc.i4.1
    IL_27c0:  add
    IL_27c1:  stloc.s    V_2
    IL_27c3:  ldelem.i4
    IL_27c4:  stloc.s    V_6
    IL_27c6:  ldloc.s    V_6
    IL_27c8:  ldc.i4.s   0
    IL_27ca:  ble        IL_1c62

    IL_27cf:  ldloc.s    V_3
    IL_27d1:  ldloc.s    V_2
    IL_27d3:  ldc.i4.1
    IL_27d4:  sub
    IL_27d5:  dup
    IL_27d6:  stloc.s    V_2
    IL_27d8:  ldloc.s    V_6
    IL_27da:  ldc.i4.s   1
    IL_27dc:  sub
    IL_27dd:  stelem.i4
    IL_27de:  ldloc.s    V_3
    IL_27e0:  ldloc.s    V_2
    IL_27e2:  ldc.i4.1
    IL_27e3:  sub
    IL_27e4:  dup
    IL_27e5:  stloc.s    V_2
    IL_27e7:  ldloc.s    V_0
    IL_27e9:  ldc.i4.s   1
    IL_27eb:  sub
    IL_27ec:  stelem.i4
    IL_27ed:  ldloc.s    V_3
    IL_27ef:  ldloc.s    V_2
    IL_27f1:  ldc.i4.1
    IL_27f2:  sub
    IL_27f3:  dup
    IL_27f4:  stloc.s    V_2
    IL_27f6:  ldc.i4.s   51
    IL_27f8:  stelem.i4
    IL_27f9:  br         IL_1c62
  } // end of method MainCapturePatternRunner1::Go

  .method public virtual instance bool  FindFirstChar() cil managed
  {
    // Code size       110 (0x6e)
    .maxstack  8
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0006:  stloc.s    V_0
    IL_0008:  ldarg.0
    IL_0009:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_000e:  stloc.s    V_1
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_0016:  ldloc.s    V_0
    IL_0018:  sub
    IL_0019:  stloc.s    V_3
    IL_001b:  ldloc.s    V_3
    IL_001d:  ldc.i4.s   0
    IL_001f:  ble        IL_006b

    IL_0024:  ldloc.s    V_3
    IL_0026:  ldc.i4.s   1
    IL_0028:  sub
    IL_0029:  stloc.s    V_3
    IL_002b:  ldloc.s    V_1
    IL_002d:  ldloc.s    V_0
    IL_002f:  dup
    IL_0030:  ldc.i4.s   1
    IL_0032:  add
    IL_0033:  stloc.s    V_0
    IL_0035:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_003a:  ldstr      bytearray (00 00 04 00 00 00 24 00 25 00 7D 00 7E 00 )       // ......$.%.}.~.
    IL_003f:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0044:  brtrue     IL_0059

    IL_0049:  ldloc.s    V_3
    IL_004b:  ldc.i4.s   0
    IL_004d:  bgt        IL_0024

    IL_0052:  ldc.i4.s   0
    IL_0054:  br         IL_0062

    IL_0059:  ldloc.s    V_0
    IL_005b:  ldc.i4.s   1
    IL_005d:  sub
    IL_005e:  stloc.s    V_0
    IL_0060:  ldc.i4.s   1
    IL_0062:  ldarg.0
    IL_0063:  ldloc.s    V_0
    IL_0065:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_006a:  ret

    IL_006b:  ldc.i4.s   0
    IL_006d:  ret
  } // end of method MainCapturePatternRunner1::FindFirstChar

  .method public virtual instance void  InitTrackCount() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   98
    IL_0003:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackcount
    IL_0008:  ret
  } // end of method MainCapturePatternRunner1::InitTrackCount

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunner::.ctor()
    IL_0006:  ret
  } // end of method MainCapturePatternRunner1::.ctor

} // end of class Xeora.Web.RegularExpression.MainCapturePatternRunner1

.class private auto ansi Xeora.Web.RegularExpression.MainCapturePatternFactory1
       extends [System]System.Text.RegularExpressions.RegexRunnerFactory
{
  .method public virtual instance class [System]System.Text.RegularExpressions.RegexRunner 
          CreateInstance() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  1
    IL_0000:  newobj     instance void Xeora.Web.RegularExpression.MainCapturePatternRunner1::.ctor()
    IL_0005:  ret
  } // end of method MainCapturePatternFactory1::CreateInstance

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunnerFactory::.ctor()
    IL_0006:  ret
  } // end of method MainCapturePatternFactory1::.ctor

} // end of class Xeora.Web.RegularExpression.MainCapturePatternFactory1

.class public auto ansi Xeora.Web.RegularExpression.MainCapturePattern
       extends [System]System.Text.RegularExpressions.Regex
{
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       51 (0x33)
    .maxstack  4
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.Regex::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldstr      "\\$((([#]+|[\\^\\-\\+\\*\\~])\?(\\w+)|@[#\\-]*(\\w+\\.)[\\w+\\."
    + "]+)\\$|[\\.\\w\\-]+\\:\\{|\\w(\\#\\d+(\\+)\?)\?(\\[[\\.\\w\\-]+\\])\?\\:([\\/\\.\\w\\-]+\\$|[\\."
    + "\\w\\-]+\\:\\{|[\\.\\w\\-]+\\\?[\\.\\w\\-]+(\\,((\\|)\?([#\\.\\^\\-\\+\\*\\~]*([\\w+][^\\$]*)|"
    + "\\=([\\S+][^\\$]*)|@[#\\-]*(\\w+\\.)[\\w+\\.]+)\?)*)\?\\$))|\\}\\:[\\.\\w\\-]+\\:\\{|\\}\\:"
    + "[\\.\\w\\-]+\\$"
    IL_000c:  stfld      string [System]System.Text.RegularExpressions.Regex::pattern
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.s   2
    IL_0014:  stfld      valuetype [System]System.Text.RegularExpressions.RegexOptions [System]System.Text.RegularExpressions.Regex::roptions
    IL_0019:  ldarg.0
    IL_001a:  newobj     instance void Xeora.Web.RegularExpression.MainCapturePatternFactory1::.ctor()
    IL_001f:  stfld      class [System]System.Text.RegularExpressions.RegexRunnerFactory [System]System.Text.RegularExpressions.Regex::factory
    IL_0024:  ldarg.0
    IL_0025:  ldc.i4.s   17
    IL_0027:  stfld      int32 [System]System.Text.RegularExpressions.Regex::capsize
    IL_002c:  ldarg.0
    IL_002d:  call       instance void [System]System.Text.RegularExpressions.Regex::InitializeReferences()
    IL_0032:  ret
  } // end of method MainCapturePattern::.ctor

} // end of class Xeora.Web.RegularExpression.MainCapturePattern

.class private auto ansi Xeora.Web.RegularExpression.BracketedControllerOpenPatternRunner2
       extends [System]System.Text.RegularExpressions.RegexRunner
{
  .method public virtual instance void  Go() cil managed
  {
    // Code size       3159 (0xc57)
    .maxstack  105
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32[] V_3,
             int32 V_4,
             int32[] V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10,
             int32 V_11)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextstart
    IL_000e:  stloc.s    V_11
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextbeg
    IL_0016:  stloc.s    V_9
    IL_0018:  ldarg.0
    IL_0019:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_001e:  stloc.s    V_10
    IL_0020:  ldarg.0
    IL_0021:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0026:  stloc.s    V_0
    IL_0028:  ldarg.0
    IL_0029:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_002e:  stloc.s    V_3
    IL_0030:  ldarg.0
    IL_0031:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_0036:  stloc.s    V_2
    IL_0038:  ldarg.0
    IL_0039:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_003e:  stloc.s    V_5
    IL_0040:  ldarg.0
    IL_0041:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0046:  stloc.s    V_4
    IL_0048:  ldloc.s    V_3
    IL_004a:  ldloc.s    V_2
    IL_004c:  ldc.i4.1
    IL_004d:  sub
    IL_004e:  dup
    IL_004f:  stloc.s    V_2
    IL_0051:  ldloc.s    V_0
    IL_0053:  stelem.i4
    IL_0054:  ldloc.s    V_3
    IL_0056:  ldloc.s    V_2
    IL_0058:  ldc.i4.1
    IL_0059:  sub
    IL_005a:  dup
    IL_005b:  stloc.s    V_2
    IL_005d:  ldc.i4.s   0
    IL_005f:  stelem.i4
    IL_0060:  ldloc.s    V_5
    IL_0062:  ldloc.s    V_4
    IL_0064:  ldc.i4.1
    IL_0065:  sub
    IL_0066:  dup
    IL_0067:  stloc.s    V_4
    IL_0069:  ldloc.s    V_0
    IL_006b:  stelem.i4
    IL_006c:  ldloc.s    V_3
    IL_006e:  ldloc.s    V_2
    IL_0070:  ldc.i4.1
    IL_0071:  sub
    IL_0072:  dup
    IL_0073:  stloc.s    V_2
    IL_0075:  ldc.i4.s   1
    IL_0077:  stelem.i4
    IL_0078:  ldloc.s    V_0
    IL_007a:  ldloc.s    V_10
    IL_007c:  bge        IL_08f1

    IL_0081:  ldloc.s    V_1
    IL_0083:  ldloc.s    V_0
    IL_0085:  dup
    IL_0086:  ldc.i4.s   1
    IL_0088:  add
    IL_0089:  stloc.s    V_0
    IL_008b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0090:  ldc.i4.s   36
    IL_0092:  bne.un     IL_08f1

    IL_0097:  ldloc.s    V_5
    IL_0099:  ldloc.s    V_4
    IL_009b:  ldc.i4.1
    IL_009c:  sub
    IL_009d:  dup
    IL_009e:  stloc.s    V_4
    IL_00a0:  ldloc.s    V_0
    IL_00a2:  stelem.i4
    IL_00a3:  ldloc.s    V_3
    IL_00a5:  ldloc.s    V_2
    IL_00a7:  ldc.i4.1
    IL_00a8:  sub
    IL_00a9:  dup
    IL_00aa:  stloc.s    V_2
    IL_00ac:  ldc.i4.s   1
    IL_00ae:  stelem.i4
    IL_00af:  ldloc.s    V_3
    IL_00b1:  ldloc.s    V_2
    IL_00b3:  ldc.i4.1
    IL_00b4:  sub
    IL_00b5:  dup
    IL_00b6:  stloc.s    V_2
    IL_00b8:  ldloc.s    V_0
    IL_00ba:  stelem.i4
    IL_00bb:  ldloc.s    V_3
    IL_00bd:  ldloc.s    V_2
    IL_00bf:  ldc.i4.1
    IL_00c0:  sub
    IL_00c1:  dup
    IL_00c2:  stloc.s    V_2
    IL_00c4:  ldc.i4.s   2
    IL_00c6:  stelem.i4
    IL_00c7:  ldloc.s    V_5
    IL_00c9:  ldloc.s    V_4
    IL_00cb:  ldc.i4.1
    IL_00cc:  sub
    IL_00cd:  dup
    IL_00ce:  stloc.s    V_4
    IL_00d0:  ldloc.s    V_0
    IL_00d2:  stelem.i4
    IL_00d3:  ldloc.s    V_3
    IL_00d5:  ldloc.s    V_2
    IL_00d7:  ldc.i4.1
    IL_00d8:  sub
    IL_00d9:  dup
    IL_00da:  stloc.s    V_2
    IL_00dc:  ldc.i4.s   1
    IL_00de:  stelem.i4
    IL_00df:  ldc.i4.s   1
    IL_00e1:  ldloc.s    V_10
    IL_00e3:  ldloc.s    V_0
    IL_00e5:  sub
    IL_00e6:  bgt        IL_08f1

    IL_00eb:  ldloc.s    V_0
    IL_00ed:  ldc.i4.s   1
    IL_00ef:  add
    IL_00f0:  stloc.s    V_0
    IL_00f2:  ldc.i4.s   1
    IL_00f4:  stloc.s    V_6
    IL_00f6:  ldloc.s    V_1
    IL_00f8:  ldloc.s    V_0
    IL_00fa:  ldloc.s    V_6
    IL_00fc:  dup
    IL_00fd:  ldc.i4.s   1
    IL_00ff:  sub
    IL_0100:  stloc.s    V_6
    IL_0102:  sub
    IL_0103:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0108:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_010d:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0112:  brfalse    IL_08f1

    IL_0117:  ldloc.s    V_6
    IL_0119:  ldc.i4.s   0
    IL_011b:  bgt        IL_00f6

    IL_0120:  ldloc.s    V_10
    IL_0122:  ldloc.s    V_0
    IL_0124:  sub
    IL_0125:  dup
    IL_0126:  stloc.s    V_7
    IL_0128:  ldc.i4.s   1
    IL_012a:  add
    IL_012b:  stloc.s    V_6
    IL_012d:  ldloc.s    V_6
    IL_012f:  ldc.i4.s   1
    IL_0131:  sub
    IL_0132:  dup
    IL_0133:  stloc.s    V_6
    IL_0135:  ldc.i4.s   0
    IL_0137:  ble        IL_0161

    IL_013c:  ldloc.s    V_1
    IL_013e:  ldloc.s    V_0
    IL_0140:  dup
    IL_0141:  ldc.i4.s   1
    IL_0143:  add
    IL_0144:  stloc.s    V_0
    IL_0146:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_014b:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_0150:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0155:  brtrue     IL_012d

    IL_015a:  ldloc.s    V_0
    IL_015c:  ldc.i4.s   1
    IL_015e:  sub
    IL_015f:  stloc.s    V_0
    IL_0161:  ldloc.s    V_7
    IL_0163:  ldloc.s    V_6
    IL_0165:  ble.s      IL_0194

    IL_0167:  ldloc.s    V_3
    IL_0169:  ldloc.s    V_2
    IL_016b:  ldc.i4.1
    IL_016c:  sub
    IL_016d:  dup
    IL_016e:  stloc.s    V_2
    IL_0170:  ldloc.s    V_7
    IL_0172:  ldloc.s    V_6
    IL_0174:  sub
    IL_0175:  ldc.i4.s   1
    IL_0177:  sub
    IL_0178:  stelem.i4
    IL_0179:  ldloc.s    V_3
    IL_017b:  ldloc.s    V_2
    IL_017d:  ldc.i4.1
    IL_017e:  sub
    IL_017f:  dup
    IL_0180:  stloc.s    V_2
    IL_0182:  ldloc.s    V_0
    IL_0184:  ldc.i4.s   1
    IL_0186:  sub
    IL_0187:  stelem.i4
    IL_0188:  ldloc.s    V_3
    IL_018a:  ldloc.s    V_2
    IL_018c:  ldc.i4.1
    IL_018d:  sub
    IL_018e:  dup
    IL_018f:  stloc.s    V_2
    IL_0191:  ldc.i4.s   3
    IL_0193:  stelem.i4
    IL_0194:  ldloc.s    V_5
    IL_0196:  ldloc.s    V_4
    IL_0198:  dup
    IL_0199:  ldc.i4.1
    IL_019a:  add
    IL_019b:  stloc.s    V_4
    IL_019d:  ldelem.i4
    IL_019e:  stloc.s    V_6
    IL_01a0:  ldarg.0
    IL_01a1:  ldc.i4.s   5
    IL_01a3:  ldloc.s    V_6
    IL_01a5:  ldloc.s    V_0
    IL_01a7:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_01ac:  ldloc.s    V_3
    IL_01ae:  ldloc.s    V_2
    IL_01b0:  ldc.i4.1
    IL_01b1:  sub
    IL_01b2:  dup
    IL_01b3:  stloc.s    V_2
    IL_01b5:  ldloc.s    V_6
    IL_01b7:  stelem.i4
    IL_01b8:  ldloc.s    V_3
    IL_01ba:  ldloc.s    V_2
    IL_01bc:  ldc.i4.1
    IL_01bd:  sub
    IL_01be:  dup
    IL_01bf:  stloc.s    V_2
    IL_01c1:  ldc.i4.s   4
    IL_01c3:  stelem.i4
    IL_01c4:  br         IL_0852

    IL_01c9:  ldloc.s    V_5
    IL_01cb:  ldloc.s    V_4
    IL_01cd:  ldc.i4.1
    IL_01ce:  sub
    IL_01cf:  dup
    IL_01d0:  stloc.s    V_4
    IL_01d2:  ldloc.s    V_0
    IL_01d4:  stelem.i4
    IL_01d5:  ldloc.s    V_3
    IL_01d7:  ldloc.s    V_2
    IL_01d9:  ldc.i4.1
    IL_01da:  sub
    IL_01db:  dup
    IL_01dc:  stloc.s    V_2
    IL_01de:  ldc.i4.s   1
    IL_01e0:  stelem.i4
    IL_01e1:  ldloc.s    V_0
    IL_01e3:  ldloc.s    V_10
    IL_01e5:  bge        IL_08f1

    IL_01ea:  ldloc.s    V_1
    IL_01ec:  ldloc.s    V_0
    IL_01ee:  dup
    IL_01ef:  ldc.i4.s   1
    IL_01f1:  add
    IL_01f2:  stloc.s    V_0
    IL_01f4:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_01f9:  ldstr      bytearray (00 00 00 00 09 00 00 00 02 00 04 00 05 00 03 00 
                                    01 00 09 00 13 00 00 00 ) 
    IL_01fe:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0203:  brfalse    IL_08f1

    IL_0208:  ldloc.s    V_5
    IL_020a:  ldloc.s    V_4
    IL_020c:  dup
    IL_020d:  ldc.i4.1
    IL_020e:  add
    IL_020f:  stloc.s    V_4
    IL_0211:  ldelem.i4
    IL_0212:  stloc.s    V_6
    IL_0214:  ldarg.0
    IL_0215:  ldc.i4.s   6
    IL_0217:  ldloc.s    V_6
    IL_0219:  ldloc.s    V_0
    IL_021b:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0220:  ldloc.s    V_3
    IL_0222:  ldloc.s    V_2
    IL_0224:  ldc.i4.1
    IL_0225:  sub
    IL_0226:  dup
    IL_0227:  stloc.s    V_2
    IL_0229:  ldloc.s    V_6
    IL_022b:  stelem.i4
    IL_022c:  ldloc.s    V_3
    IL_022e:  ldloc.s    V_2
    IL_0230:  ldc.i4.1
    IL_0231:  sub
    IL_0232:  dup
    IL_0233:  stloc.s    V_2
    IL_0235:  ldc.i4.s   4
    IL_0237:  stelem.i4
    IL_0238:  ldloc.s    V_5
    IL_023a:  ldloc.s    V_4
    IL_023c:  ldc.i4.1
    IL_023d:  sub
    IL_023e:  dup
    IL_023f:  stloc.s    V_4
    IL_0241:  ldc.i4.s   -1
    IL_0243:  stelem.i4
    IL_0244:  ldloc.s    V_5
    IL_0246:  ldloc.s    V_4
    IL_0248:  ldc.i4.1
    IL_0249:  sub
    IL_024a:  dup
    IL_024b:  stloc.s    V_4
    IL_024d:  ldc.i4.s   0
    IL_024f:  stelem.i4
    IL_0250:  ldloc.s    V_3
    IL_0252:  ldloc.s    V_2
    IL_0254:  ldc.i4.1
    IL_0255:  sub
    IL_0256:  dup
    IL_0257:  stloc.s    V_2
    IL_0259:  ldc.i4.s   5
    IL_025b:  stelem.i4
    IL_025c:  br         IL_04a4

    IL_0261:  ldloc.s    V_5
    IL_0263:  ldloc.s    V_4
    IL_0265:  ldc.i4.1
    IL_0266:  sub
    IL_0267:  dup
    IL_0268:  stloc.s    V_4
    IL_026a:  ldloc.s    V_0
    IL_026c:  stelem.i4
    IL_026d:  ldloc.s    V_3
    IL_026f:  ldloc.s    V_2
    IL_0271:  ldc.i4.1
    IL_0272:  sub
    IL_0273:  dup
    IL_0274:  stloc.s    V_2
    IL_0276:  ldc.i4.s   1
    IL_0278:  stelem.i4
    IL_0279:  ldloc.s    V_0
    IL_027b:  ldloc.s    V_10
    IL_027d:  bge        IL_08f1

    IL_0282:  ldloc.s    V_1
    IL_0284:  ldloc.s    V_0
    IL_0286:  dup
    IL_0287:  ldc.i4.s   1
    IL_0289:  add
    IL_028a:  stloc.s    V_0
    IL_028c:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0291:  ldc.i4.s   35
    IL_0293:  bne.un     IL_08f1

    IL_0298:  ldc.i4.s   1
    IL_029a:  ldloc.s    V_10
    IL_029c:  ldloc.s    V_0
    IL_029e:  sub
    IL_029f:  bgt        IL_08f1

    IL_02a4:  ldloc.s    V_0
    IL_02a6:  ldc.i4.s   1
    IL_02a8:  add
    IL_02a9:  stloc.s    V_0
    IL_02ab:  ldc.i4.s   1
    IL_02ad:  stloc.s    V_6
    IL_02af:  ldloc.s    V_1
    IL_02b1:  ldloc.s    V_0
    IL_02b3:  ldloc.s    V_6
    IL_02b5:  dup
    IL_02b6:  ldc.i4.s   1
    IL_02b8:  sub
    IL_02b9:  stloc.s    V_6
    IL_02bb:  sub
    IL_02bc:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_02c1:  ldstr      bytearray (00 00 00 00 01 00 09 00 ) 
    IL_02c6:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_02cb:  brfalse    IL_08f1

    IL_02d0:  ldloc.s    V_6
    IL_02d2:  ldc.i4.s   0
    IL_02d4:  bgt        IL_02af

    IL_02d9:  ldloc.s    V_10
    IL_02db:  ldloc.s    V_0
    IL_02dd:  sub
    IL_02de:  dup
    IL_02df:  stloc.s    V_7
    IL_02e1:  ldc.i4.s   1
    IL_02e3:  add
    IL_02e4:  stloc.s    V_6
    IL_02e6:  ldloc.s    V_6
    IL_02e8:  ldc.i4.s   1
    IL_02ea:  sub
    IL_02eb:  dup
    IL_02ec:  stloc.s    V_6
    IL_02ee:  ldc.i4.s   0
    IL_02f0:  ble        IL_031a

    IL_02f5:  ldloc.s    V_1
    IL_02f7:  ldloc.s    V_0
    IL_02f9:  dup
    IL_02fa:  ldc.i4.s   1
    IL_02fc:  add
    IL_02fd:  stloc.s    V_0
    IL_02ff:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0304:  ldstr      bytearray (00 00 00 00 01 00 09 00 ) 
    IL_0309:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_030e:  brtrue     IL_02e6

    IL_0313:  ldloc.s    V_0
    IL_0315:  ldc.i4.s   1
    IL_0317:  sub
    IL_0318:  stloc.s    V_0
    IL_031a:  ldloc.s    V_7
    IL_031c:  ldloc.s    V_6
    IL_031e:  ble.s      IL_034d

    IL_0320:  ldloc.s    V_3
    IL_0322:  ldloc.s    V_2
    IL_0324:  ldc.i4.1
    IL_0325:  sub
    IL_0326:  dup
    IL_0327:  stloc.s    V_2
    IL_0329:  ldloc.s    V_7
    IL_032b:  ldloc.s    V_6
    IL_032d:  sub
    IL_032e:  ldc.i4.s   1
    IL_0330:  sub
    IL_0331:  stelem.i4
    IL_0332:  ldloc.s    V_3
    IL_0334:  ldloc.s    V_2
    IL_0336:  ldc.i4.1
    IL_0337:  sub
    IL_0338:  dup
    IL_0339:  stloc.s    V_2
    IL_033b:  ldloc.s    V_0
    IL_033d:  ldc.i4.s   1
    IL_033f:  sub
    IL_0340:  stelem.i4
    IL_0341:  ldloc.s    V_3
    IL_0343:  ldloc.s    V_2
    IL_0345:  ldc.i4.1
    IL_0346:  sub
    IL_0347:  dup
    IL_0348:  stloc.s    V_2
    IL_034a:  ldc.i4.s   6
    IL_034c:  stelem.i4
    IL_034d:  ldloc.s    V_5
    IL_034f:  ldloc.s    V_4
    IL_0351:  ldc.i4.1
    IL_0352:  sub
    IL_0353:  dup
    IL_0354:  stloc.s    V_4
    IL_0356:  ldc.i4.s   -1
    IL_0358:  stelem.i4
    IL_0359:  ldloc.s    V_5
    IL_035b:  ldloc.s    V_4
    IL_035d:  ldc.i4.1
    IL_035e:  sub
    IL_035f:  dup
    IL_0360:  stloc.s    V_4
    IL_0362:  ldc.i4.s   0
    IL_0364:  stelem.i4
    IL_0365:  ldloc.s    V_3
    IL_0367:  ldloc.s    V_2
    IL_0369:  ldc.i4.1
    IL_036a:  sub
    IL_036b:  dup
    IL_036c:  stloc.s    V_2
    IL_036e:  ldc.i4.s   5
    IL_0370:  stelem.i4
    IL_0371:  br         IL_03dd

    IL_0376:  ldloc.s    V_5
    IL_0378:  ldloc.s    V_4
    IL_037a:  ldc.i4.1
    IL_037b:  sub
    IL_037c:  dup
    IL_037d:  stloc.s    V_4
    IL_037f:  ldloc.s    V_0
    IL_0381:  stelem.i4
    IL_0382:  ldloc.s    V_3
    IL_0384:  ldloc.s    V_2
    IL_0386:  ldc.i4.1
    IL_0387:  sub
    IL_0388:  dup
    IL_0389:  stloc.s    V_2
    IL_038b:  ldc.i4.s   1
    IL_038d:  stelem.i4
    IL_038e:  ldloc.s    V_0
    IL_0390:  ldloc.s    V_10
    IL_0392:  bge        IL_08f1

    IL_0397:  ldloc.s    V_1
    IL_0399:  ldloc.s    V_0
    IL_039b:  dup
    IL_039c:  ldc.i4.s   1
    IL_039e:  add
    IL_039f:  stloc.s    V_0
    IL_03a1:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_03a6:  ldc.i4.s   43
    IL_03a8:  bne.un     IL_08f1

    IL_03ad:  ldloc.s    V_5
    IL_03af:  ldloc.s    V_4
    IL_03b1:  dup
    IL_03b2:  ldc.i4.1
    IL_03b3:  add
    IL_03b4:  stloc.s    V_4
    IL_03b6:  ldelem.i4
    IL_03b7:  stloc.s    V_6
    IL_03b9:  ldarg.0
    IL_03ba:  ldc.i4.s   3
    IL_03bc:  ldloc.s    V_6
    IL_03be:  ldloc.s    V_0
    IL_03c0:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_03c5:  ldloc.s    V_3
    IL_03c7:  ldloc.s    V_2
    IL_03c9:  ldc.i4.1
    IL_03ca:  sub
    IL_03cb:  dup
    IL_03cc:  stloc.s    V_2
    IL_03ce:  ldloc.s    V_6
    IL_03d0:  stelem.i4
    IL_03d1:  ldloc.s    V_3
    IL_03d3:  ldloc.s    V_2
    IL_03d5:  ldc.i4.1
    IL_03d6:  sub
    IL_03d7:  dup
    IL_03d8:  stloc.s    V_2
    IL_03da:  ldc.i4.s   4
    IL_03dc:  stelem.i4
    IL_03dd:  ldloc.s    V_5
    IL_03df:  ldloc.s    V_4
    IL_03e1:  dup
    IL_03e2:  ldc.i4.1
    IL_03e3:  add
    IL_03e4:  stloc.s    V_4
    IL_03e6:  ldelem.i4
    IL_03e7:  stloc.s    V_6
    IL_03e9:  ldloc.s    V_5
    IL_03eb:  ldloc.s    V_4
    IL_03ed:  dup
    IL_03ee:  ldc.i4.1
    IL_03ef:  add
    IL_03f0:  stloc.s    V_4
    IL_03f2:  ldelem.i4
    IL_03f3:  dup
    IL_03f4:  stloc.s    V_7
    IL_03f6:  ldloc.s    V_3
    IL_03f8:  ldloc.s    V_2
    IL_03fa:  ldc.i4.1
    IL_03fb:  sub
    IL_03fc:  dup
    IL_03fd:  stloc.s    V_2
    IL_03ff:  ldloc.s    V_7
    IL_0401:  stelem.i4
    IL_0402:  ldloc.s    V_0
    IL_0404:  bne.un.s   IL_040c

    IL_0406:  ldloc.s    V_6
    IL_0408:  ldc.i4.s   0
    IL_040a:  bge.s      IL_045c

    IL_040c:  ldloc.s    V_6
    IL_040e:  ldc.i4.s   1
    IL_0410:  bge.s      IL_045c

    IL_0412:  ldloc.s    V_5
    IL_0414:  ldloc.s    V_4
    IL_0416:  ldc.i4.1
    IL_0417:  sub
    IL_0418:  dup
    IL_0419:  stloc.s    V_4
    IL_041b:  ldloc.s    V_0
    IL_041d:  stelem.i4
    IL_041e:  ldloc.s    V_5
    IL_0420:  ldloc.s    V_4
    IL_0422:  ldc.i4.1
    IL_0423:  sub
    IL_0424:  dup
    IL_0425:  stloc.s    V_4
    IL_0427:  ldloc.s    V_6
    IL_0429:  ldc.i4.s   1
    IL_042b:  add
    IL_042c:  stelem.i4
    IL_042d:  ldloc.s    V_3
    IL_042f:  ldloc.s    V_2
    IL_0431:  ldc.i4.1
    IL_0432:  sub
    IL_0433:  dup
    IL_0434:  stloc.s    V_2
    IL_0436:  ldc.i4.s   7
    IL_0438:  stelem.i4
    IL_0439:  ldloc.s    V_2
    IL_043b:  ldc.i4     0x80
    IL_0440:  ble.s      IL_044b

    IL_0442:  ldloc.s    V_4
    IL_0444:  ldc.i4.s   96
    IL_0446:  bgt        IL_0376

    IL_044b:  ldloc.s    V_3
    IL_044d:  ldloc.s    V_2
    IL_044f:  ldc.i4.1
    IL_0450:  sub
    IL_0451:  dup
    IL_0452:  stloc.s    V_2
    IL_0454:  ldc.i4.s   8
    IL_0456:  stelem.i4
    IL_0457:  br         IL_08f1

    IL_045c:  ldloc.s    V_3
    IL_045e:  ldloc.s    V_2
    IL_0460:  ldc.i4.1
    IL_0461:  sub
    IL_0462:  dup
    IL_0463:  stloc.s    V_2
    IL_0465:  ldloc.s    V_6
    IL_0467:  stelem.i4
    IL_0468:  ldloc.s    V_3
    IL_046a:  ldloc.s    V_2
    IL_046c:  ldc.i4.1
    IL_046d:  sub
    IL_046e:  dup
    IL_046f:  stloc.s    V_2
    IL_0471:  ldc.i4.s   9
    IL_0473:  stelem.i4
    IL_0474:  ldloc.s    V_5
    IL_0476:  ldloc.s    V_4
    IL_0478:  dup
    IL_0479:  ldc.i4.1
    IL_047a:  add
    IL_047b:  stloc.s    V_4
    IL_047d:  ldelem.i4
    IL_047e:  stloc.s    V_6
    IL_0480:  ldarg.0
    IL_0481:  ldc.i4.s   2
    IL_0483:  ldloc.s    V_6
    IL_0485:  ldloc.s    V_0
    IL_0487:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_048c:  ldloc.s    V_3
    IL_048e:  ldloc.s    V_2
    IL_0490:  ldc.i4.1
    IL_0491:  sub
    IL_0492:  dup
    IL_0493:  stloc.s    V_2
    IL_0495:  ldloc.s    V_6
    IL_0497:  stelem.i4
    IL_0498:  ldloc.s    V_3
    IL_049a:  ldloc.s    V_2
    IL_049c:  ldc.i4.1
    IL_049d:  sub
    IL_049e:  dup
    IL_049f:  stloc.s    V_2
    IL_04a1:  ldc.i4.s   4
    IL_04a3:  stelem.i4
    IL_04a4:  ldloc.s    V_5
    IL_04a6:  ldloc.s    V_4
    IL_04a8:  dup
    IL_04a9:  ldc.i4.1
    IL_04aa:  add
    IL_04ab:  stloc.s    V_4
    IL_04ad:  ldelem.i4
    IL_04ae:  stloc.s    V_6
    IL_04b0:  ldloc.s    V_5
    IL_04b2:  ldloc.s    V_4
    IL_04b4:  dup
    IL_04b5:  ldc.i4.1
    IL_04b6:  add
    IL_04b7:  stloc.s    V_4
    IL_04b9:  ldelem.i4
    IL_04ba:  dup
    IL_04bb:  stloc.s    V_7
    IL_04bd:  ldloc.s    V_3
    IL_04bf:  ldloc.s    V_2
    IL_04c1:  ldc.i4.1
    IL_04c2:  sub
    IL_04c3:  dup
    IL_04c4:  stloc.s    V_2
    IL_04c6:  ldloc.s    V_7
    IL_04c8:  stelem.i4
    IL_04c9:  ldloc.s    V_0
    IL_04cb:  bne.un.s   IL_04d3

    IL_04cd:  ldloc.s    V_6
    IL_04cf:  ldc.i4.s   0
    IL_04d1:  bge.s      IL_0523

    IL_04d3:  ldloc.s    V_6
    IL_04d5:  ldc.i4.s   1
    IL_04d7:  bge.s      IL_0523

    IL_04d9:  ldloc.s    V_5
    IL_04db:  ldloc.s    V_4
    IL_04dd:  ldc.i4.1
    IL_04de:  sub
    IL_04df:  dup
    IL_04e0:  stloc.s    V_4
    IL_04e2:  ldloc.s    V_0
    IL_04e4:  stelem.i4
    IL_04e5:  ldloc.s    V_5
    IL_04e7:  ldloc.s    V_4
    IL_04e9:  ldc.i4.1
    IL_04ea:  sub
    IL_04eb:  dup
    IL_04ec:  stloc.s    V_4
    IL_04ee:  ldloc.s    V_6
    IL_04f0:  ldc.i4.s   1
    IL_04f2:  add
    IL_04f3:  stelem.i4
    IL_04f4:  ldloc.s    V_3
    IL_04f6:  ldloc.s    V_2
    IL_04f8:  ldc.i4.1
    IL_04f9:  sub
    IL_04fa:  dup
    IL_04fb:  stloc.s    V_2
    IL_04fd:  ldc.i4.s   10
    IL_04ff:  stelem.i4
    IL_0500:  ldloc.s    V_2
    IL_0502:  ldc.i4     0x80
    IL_0507:  ble.s      IL_0512

    IL_0509:  ldloc.s    V_4
    IL_050b:  ldc.i4.s   96
    IL_050d:  bgt        IL_0261

    IL_0512:  ldloc.s    V_3
    IL_0514:  ldloc.s    V_2
    IL_0516:  ldc.i4.1
    IL_0517:  sub
    IL_0518:  dup
    IL_0519:  stloc.s    V_2
    IL_051b:  ldc.i4.s   11
    IL_051d:  stelem.i4
    IL_051e:  br         IL_08f1

    IL_0523:  ldloc.s    V_3
    IL_0525:  ldloc.s    V_2
    IL_0527:  ldc.i4.1
    IL_0528:  sub
    IL_0529:  dup
    IL_052a:  stloc.s    V_2
    IL_052c:  ldloc.s    V_6
    IL_052e:  stelem.i4
    IL_052f:  ldloc.s    V_3
    IL_0531:  ldloc.s    V_2
    IL_0533:  ldc.i4.1
    IL_0534:  sub
    IL_0535:  dup
    IL_0536:  stloc.s    V_2
    IL_0538:  ldc.i4.s   9
    IL_053a:  stelem.i4
    IL_053b:  ldloc.s    V_5
    IL_053d:  ldloc.s    V_4
    IL_053f:  ldc.i4.1
    IL_0540:  sub
    IL_0541:  dup
    IL_0542:  stloc.s    V_4
    IL_0544:  ldc.i4.s   -1
    IL_0546:  stelem.i4
    IL_0547:  ldloc.s    V_5
    IL_0549:  ldloc.s    V_4
    IL_054b:  ldc.i4.1
    IL_054c:  sub
    IL_054d:  dup
    IL_054e:  stloc.s    V_4
    IL_0550:  ldc.i4.s   0
    IL_0552:  stelem.i4
    IL_0553:  ldloc.s    V_3
    IL_0555:  ldloc.s    V_2
    IL_0557:  ldc.i4.1
    IL_0558:  sub
    IL_0559:  dup
    IL_055a:  stloc.s    V_2
    IL_055c:  ldc.i4.s   5
    IL_055e:  stelem.i4
    IL_055f:  br         IL_069f

    IL_0564:  ldloc.s    V_5
    IL_0566:  ldloc.s    V_4
    IL_0568:  ldc.i4.1
    IL_0569:  sub
    IL_056a:  dup
    IL_056b:  stloc.s    V_4
    IL_056d:  ldloc.s    V_0
    IL_056f:  stelem.i4
    IL_0570:  ldloc.s    V_3
    IL_0572:  ldloc.s    V_2
    IL_0574:  ldc.i4.1
    IL_0575:  sub
    IL_0576:  dup
    IL_0577:  stloc.s    V_2
    IL_0579:  ldc.i4.s   1
    IL_057b:  stelem.i4
    IL_057c:  ldloc.s    V_0
    IL_057e:  ldloc.s    V_10
    IL_0580:  bge        IL_08f1

    IL_0585:  ldloc.s    V_1
    IL_0587:  ldloc.s    V_0
    IL_0589:  dup
    IL_058a:  ldc.i4.s   1
    IL_058c:  add
    IL_058d:  stloc.s    V_0
    IL_058f:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0594:  ldc.i4.s   91
    IL_0596:  bne.un     IL_08f1

    IL_059b:  ldc.i4.s   1
    IL_059d:  ldloc.s    V_10
    IL_059f:  ldloc.s    V_0
    IL_05a1:  sub
    IL_05a2:  bgt        IL_08f1

    IL_05a7:  ldloc.s    V_0
    IL_05a9:  ldc.i4.s   1
    IL_05ab:  add
    IL_05ac:  stloc.s    V_0
    IL_05ae:  ldc.i4.s   1
    IL_05b0:  stloc.s    V_6
    IL_05b2:  ldloc.s    V_1
    IL_05b4:  ldloc.s    V_0
    IL_05b6:  ldloc.s    V_6
    IL_05b8:  dup
    IL_05b9:  ldc.i4.s   1
    IL_05bb:  sub
    IL_05bc:  stloc.s    V_6
    IL_05be:  sub
    IL_05bf:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_05c4:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_05c9:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_05ce:  brfalse    IL_08f1

    IL_05d3:  ldloc.s    V_6
    IL_05d5:  ldc.i4.s   0
    IL_05d7:  bgt        IL_05b2

    IL_05dc:  ldloc.s    V_10
    IL_05de:  ldloc.s    V_0
    IL_05e0:  sub
    IL_05e1:  dup
    IL_05e2:  stloc.s    V_7
    IL_05e4:  ldc.i4.s   1
    IL_05e6:  add
    IL_05e7:  stloc.s    V_6
    IL_05e9:  ldloc.s    V_6
    IL_05eb:  ldc.i4.s   1
    IL_05ed:  sub
    IL_05ee:  dup
    IL_05ef:  stloc.s    V_6
    IL_05f1:  ldc.i4.s   0
    IL_05f3:  ble        IL_061d

    IL_05f8:  ldloc.s    V_1
    IL_05fa:  ldloc.s    V_0
    IL_05fc:  dup
    IL_05fd:  ldc.i4.s   1
    IL_05ff:  add
    IL_0600:  stloc.s    V_0
    IL_0602:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0607:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_060c:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_0611:  brtrue     IL_05e9

    IL_0616:  ldloc.s    V_0
    IL_0618:  ldc.i4.s   1
    IL_061a:  sub
    IL_061b:  stloc.s    V_0
    IL_061d:  ldloc.s    V_7
    IL_061f:  ldloc.s    V_6
    IL_0621:  ble.s      IL_0650

    IL_0623:  ldloc.s    V_3
    IL_0625:  ldloc.s    V_2
    IL_0627:  ldc.i4.1
    IL_0628:  sub
    IL_0629:  dup
    IL_062a:  stloc.s    V_2
    IL_062c:  ldloc.s    V_7
    IL_062e:  ldloc.s    V_6
    IL_0630:  sub
    IL_0631:  ldc.i4.s   1
    IL_0633:  sub
    IL_0634:  stelem.i4
    IL_0635:  ldloc.s    V_3
    IL_0637:  ldloc.s    V_2
    IL_0639:  ldc.i4.1
    IL_063a:  sub
    IL_063b:  dup
    IL_063c:  stloc.s    V_2
    IL_063e:  ldloc.s    V_0
    IL_0640:  ldc.i4.s   1
    IL_0642:  sub
    IL_0643:  stelem.i4
    IL_0644:  ldloc.s    V_3
    IL_0646:  ldloc.s    V_2
    IL_0648:  ldc.i4.1
    IL_0649:  sub
    IL_064a:  dup
    IL_064b:  stloc.s    V_2
    IL_064d:  ldc.i4.s   12
    IL_064f:  stelem.i4
    IL_0650:  ldloc.s    V_0
    IL_0652:  ldloc.s    V_10
    IL_0654:  bge        IL_08f1

    IL_0659:  ldloc.s    V_1
    IL_065b:  ldloc.s    V_0
    IL_065d:  dup
    IL_065e:  ldc.i4.s   1
    IL_0660:  add
    IL_0661:  stloc.s    V_0
    IL_0663:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0668:  ldc.i4.s   93
    IL_066a:  bne.un     IL_08f1

    IL_066f:  ldloc.s    V_5
    IL_0671:  ldloc.s    V_4
    IL_0673:  dup
    IL_0674:  ldc.i4.1
    IL_0675:  add
    IL_0676:  stloc.s    V_4
    IL_0678:  ldelem.i4
    IL_0679:  stloc.s    V_6
    IL_067b:  ldarg.0
    IL_067c:  ldc.i4.s   4
    IL_067e:  ldloc.s    V_6
    IL_0680:  ldloc.s    V_0
    IL_0682:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0687:  ldloc.s    V_3
    IL_0689:  ldloc.s    V_2
    IL_068b:  ldc.i4.1
    IL_068c:  sub
    IL_068d:  dup
    IL_068e:  stloc.s    V_2
    IL_0690:  ldloc.s    V_6
    IL_0692:  stelem.i4
    IL_0693:  ldloc.s    V_3
    IL_0695:  ldloc.s    V_2
    IL_0697:  ldc.i4.1
    IL_0698:  sub
    IL_0699:  dup
    IL_069a:  stloc.s    V_2
    IL_069c:  ldc.i4.s   4
    IL_069e:  stelem.i4
    IL_069f:  ldloc.s    V_5
    IL_06a1:  ldloc.s    V_4
    IL_06a3:  dup
    IL_06a4:  ldc.i4.1
    IL_06a5:  add
    IL_06a6:  stloc.s    V_4
    IL_06a8:  ldelem.i4
    IL_06a9:  stloc.s    V_6
    IL_06ab:  ldloc.s    V_5
    IL_06ad:  ldloc.s    V_4
    IL_06af:  dup
    IL_06b0:  ldc.i4.1
    IL_06b1:  add
    IL_06b2:  stloc.s    V_4
    IL_06b4:  ldelem.i4
    IL_06b5:  dup
    IL_06b6:  stloc.s    V_7
    IL_06b8:  ldloc.s    V_3
    IL_06ba:  ldloc.s    V_2
    IL_06bc:  ldc.i4.1
    IL_06bd:  sub
    IL_06be:  dup
    IL_06bf:  stloc.s    V_2
    IL_06c1:  ldloc.s    V_7
    IL_06c3:  stelem.i4
    IL_06c4:  ldloc.s    V_0
    IL_06c6:  bne.un.s   IL_06ce

    IL_06c8:  ldloc.s    V_6
    IL_06ca:  ldc.i4.s   0
    IL_06cc:  bge.s      IL_071e

    IL_06ce:  ldloc.s    V_6
    IL_06d0:  ldc.i4.s   1
    IL_06d2:  bge.s      IL_071e

    IL_06d4:  ldloc.s    V_5
    IL_06d6:  ldloc.s    V_4
    IL_06d8:  ldc.i4.1
    IL_06d9:  sub
    IL_06da:  dup
    IL_06db:  stloc.s    V_4
    IL_06dd:  ldloc.s    V_0
    IL_06df:  stelem.i4
    IL_06e0:  ldloc.s    V_5
    IL_06e2:  ldloc.s    V_4
    IL_06e4:  ldc.i4.1
    IL_06e5:  sub
    IL_06e6:  dup
    IL_06e7:  stloc.s    V_4
    IL_06e9:  ldloc.s    V_6
    IL_06eb:  ldc.i4.s   1
    IL_06ed:  add
    IL_06ee:  stelem.i4
    IL_06ef:  ldloc.s    V_3
    IL_06f1:  ldloc.s    V_2
    IL_06f3:  ldc.i4.1
    IL_06f4:  sub
    IL_06f5:  dup
    IL_06f6:  stloc.s    V_2
    IL_06f8:  ldc.i4.s   13
    IL_06fa:  stelem.i4
    IL_06fb:  ldloc.s    V_2
    IL_06fd:  ldc.i4     0x80
    IL_0702:  ble.s      IL_070d

    IL_0704:  ldloc.s    V_4
    IL_0706:  ldc.i4.s   96
    IL_0708:  bgt        IL_0564

    IL_070d:  ldloc.s    V_3
    IL_070f:  ldloc.s    V_2
    IL_0711:  ldc.i4.1
    IL_0712:  sub
    IL_0713:  dup
    IL_0714:  stloc.s    V_2
    IL_0716:  ldc.i4.s   14
    IL_0718:  stelem.i4
    IL_0719:  br         IL_08f1

    IL_071e:  ldloc.s    V_3
    IL_0720:  ldloc.s    V_2
    IL_0722:  ldc.i4.1
    IL_0723:  sub
    IL_0724:  dup
    IL_0725:  stloc.s    V_2
    IL_0727:  ldloc.s    V_6
    IL_0729:  stelem.i4
    IL_072a:  ldloc.s    V_3
    IL_072c:  ldloc.s    V_2
    IL_072e:  ldc.i4.1
    IL_072f:  sub
    IL_0730:  dup
    IL_0731:  stloc.s    V_2
    IL_0733:  ldc.i4.s   9
    IL_0735:  stelem.i4
    IL_0736:  ldloc.s    V_0
    IL_0738:  ldloc.s    V_10
    IL_073a:  bge        IL_08f1

    IL_073f:  ldloc.s    V_1
    IL_0741:  ldloc.s    V_0
    IL_0743:  dup
    IL_0744:  ldc.i4.s   1
    IL_0746:  add
    IL_0747:  stloc.s    V_0
    IL_0749:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_074e:  ldc.i4.s   58
    IL_0750:  bne.un     IL_08f1

    IL_0755:  ldloc.s    V_5
    IL_0757:  ldloc.s    V_4
    IL_0759:  ldc.i4.1
    IL_075a:  sub
    IL_075b:  dup
    IL_075c:  stloc.s    V_4
    IL_075e:  ldloc.s    V_0
    IL_0760:  stelem.i4
    IL_0761:  ldloc.s    V_3
    IL_0763:  ldloc.s    V_2
    IL_0765:  ldc.i4.1
    IL_0766:  sub
    IL_0767:  dup
    IL_0768:  stloc.s    V_2
    IL_076a:  ldc.i4.s   1
    IL_076c:  stelem.i4
    IL_076d:  ldc.i4.s   1
    IL_076f:  ldloc.s    V_10
    IL_0771:  ldloc.s    V_0
    IL_0773:  sub
    IL_0774:  bgt        IL_08f1

    IL_0779:  ldloc.s    V_0
    IL_077b:  ldc.i4.s   1
    IL_077d:  add
    IL_077e:  stloc.s    V_0
    IL_0780:  ldc.i4.s   1
    IL_0782:  stloc.s    V_6
    IL_0784:  ldloc.s    V_1
    IL_0786:  ldloc.s    V_0
    IL_0788:  ldloc.s    V_6
    IL_078a:  dup
    IL_078b:  ldc.i4.s   1
    IL_078d:  sub
    IL_078e:  stloc.s    V_6
    IL_0790:  sub
    IL_0791:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0796:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_079b:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_07a0:  brfalse    IL_08f1

    IL_07a5:  ldloc.s    V_6
    IL_07a7:  ldc.i4.s   0
    IL_07a9:  bgt        IL_0784

    IL_07ae:  ldloc.s    V_10
    IL_07b0:  ldloc.s    V_0
    IL_07b2:  sub
    IL_07b3:  dup
    IL_07b4:  stloc.s    V_7
    IL_07b6:  ldc.i4.s   1
    IL_07b8:  add
    IL_07b9:  stloc.s    V_6
    IL_07bb:  ldloc.s    V_6
    IL_07bd:  ldc.i4.s   1
    IL_07bf:  sub
    IL_07c0:  dup
    IL_07c1:  stloc.s    V_6
    IL_07c3:  ldc.i4.s   0
    IL_07c5:  ble        IL_07ef

    IL_07ca:  ldloc.s    V_1
    IL_07cc:  ldloc.s    V_0
    IL_07ce:  dup
    IL_07cf:  ldc.i4.s   1
    IL_07d1:  add
    IL_07d2:  stloc.s    V_0
    IL_07d4:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_07d9:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_07de:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_07e3:  brtrue     IL_07bb

    IL_07e8:  ldloc.s    V_0
    IL_07ea:  ldc.i4.s   1
    IL_07ec:  sub
    IL_07ed:  stloc.s    V_0
    IL_07ef:  ldloc.s    V_7
    IL_07f1:  ldloc.s    V_6
    IL_07f3:  ble.s      IL_0822

    IL_07f5:  ldloc.s    V_3
    IL_07f7:  ldloc.s    V_2
    IL_07f9:  ldc.i4.1
    IL_07fa:  sub
    IL_07fb:  dup
    IL_07fc:  stloc.s    V_2
    IL_07fe:  ldloc.s    V_7
    IL_0800:  ldloc.s    V_6
    IL_0802:  sub
    IL_0803:  ldc.i4.s   1
    IL_0805:  sub
    IL_0806:  stelem.i4
    IL_0807:  ldloc.s    V_3
    IL_0809:  ldloc.s    V_2
    IL_080b:  ldc.i4.1
    IL_080c:  sub
    IL_080d:  dup
    IL_080e:  stloc.s    V_2
    IL_0810:  ldloc.s    V_0
    IL_0812:  ldc.i4.s   1
    IL_0814:  sub
    IL_0815:  stelem.i4
    IL_0816:  ldloc.s    V_3
    IL_0818:  ldloc.s    V_2
    IL_081a:  ldc.i4.1
    IL_081b:  sub
    IL_081c:  dup
    IL_081d:  stloc.s    V_2
    IL_081f:  ldc.i4.s   15
    IL_0821:  stelem.i4
    IL_0822:  ldloc.s    V_5
    IL_0824:  ldloc.s    V_4
    IL_0826:  dup
    IL_0827:  ldc.i4.1
    IL_0828:  add
    IL_0829:  stloc.s    V_4
    IL_082b:  ldelem.i4
    IL_082c:  stloc.s    V_6
    IL_082e:  ldarg.0
    IL_082f:  ldc.i4.s   5
    IL_0831:  ldloc.s    V_6
    IL_0833:  ldloc.s    V_0
    IL_0835:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_083a:  ldloc.s    V_3
    IL_083c:  ldloc.s    V_2
    IL_083e:  ldc.i4.1
    IL_083f:  sub
    IL_0840:  dup
    IL_0841:  stloc.s    V_2
    IL_0843:  ldloc.s    V_6
    IL_0845:  stelem.i4
    IL_0846:  ldloc.s    V_3
    IL_0848:  ldloc.s    V_2
    IL_084a:  ldc.i4.1
    IL_084b:  sub
    IL_084c:  dup
    IL_084d:  stloc.s    V_2
    IL_084f:  ldc.i4.s   4
    IL_0851:  stelem.i4
    IL_0852:  ldloc.s    V_5
    IL_0854:  ldloc.s    V_4
    IL_0856:  dup
    IL_0857:  ldc.i4.1
    IL_0858:  add
    IL_0859:  stloc.s    V_4
    IL_085b:  ldelem.i4
    IL_085c:  stloc.s    V_6
    IL_085e:  ldarg.0
    IL_085f:  ldc.i4.s   1
    IL_0861:  ldloc.s    V_6
    IL_0863:  ldloc.s    V_0
    IL_0865:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_086a:  ldloc.s    V_3
    IL_086c:  ldloc.s    V_2
    IL_086e:  ldc.i4.1
    IL_086f:  sub
    IL_0870:  dup
    IL_0871:  stloc.s    V_2
    IL_0873:  ldloc.s    V_6
    IL_0875:  stelem.i4
    IL_0876:  ldloc.s    V_3
    IL_0878:  ldloc.s    V_2
    IL_087a:  ldc.i4.1
    IL_087b:  sub
    IL_087c:  dup
    IL_087d:  stloc.s    V_2
    IL_087f:  ldc.i4.s   4
    IL_0881:  stelem.i4
    IL_0882:  ldc.i4.s   2
    IL_0884:  ldloc.s    V_10
    IL_0886:  ldloc.s    V_0
    IL_0888:  sub
    IL_0889:  bgt        IL_08f1

    IL_088e:  ldloc.s    V_1
    IL_0890:  ldloc.s    V_0
    IL_0892:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0897:  ldc.i4.s   58
    IL_0899:  bne.un     IL_08f1

    IL_089e:  ldloc.s    V_1
    IL_08a0:  ldloc.s    V_0
    IL_08a2:  ldc.i4.s   1
    IL_08a4:  add
    IL_08a5:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_08aa:  ldc.i4.s   123
    IL_08ac:  bne.un     IL_08f1

    IL_08b1:  ldloc.s    V_0
    IL_08b3:  ldc.i4.s   2
    IL_08b5:  add
    IL_08b6:  stloc.s    V_0
    IL_08b8:  ldloc.s    V_5
    IL_08ba:  ldloc.s    V_4
    IL_08bc:  dup
    IL_08bd:  ldc.i4.1
    IL_08be:  add
    IL_08bf:  stloc.s    V_4
    IL_08c1:  ldelem.i4
    IL_08c2:  stloc.s    V_6
    IL_08c4:  ldarg.0
    IL_08c5:  ldc.i4.s   0
    IL_08c7:  ldloc.s    V_6
    IL_08c9:  ldloc.s    V_0
    IL_08cb:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_08d0:  ldloc.s    V_3
    IL_08d2:  ldloc.s    V_2
    IL_08d4:  ldc.i4.1
    IL_08d5:  sub
    IL_08d6:  dup
    IL_08d7:  stloc.s    V_2
    IL_08d9:  ldloc.s    V_6
    IL_08db:  stelem.i4
    IL_08dc:  ldloc.s    V_3
    IL_08de:  ldloc.s    V_2
    IL_08e0:  ldc.i4.1
    IL_08e1:  sub
    IL_08e2:  dup
    IL_08e3:  stloc.s    V_2
    IL_08e5:  ldc.i4.s   4
    IL_08e7:  stelem.i4
    IL_08e8:  ldarg.0
    IL_08e9:  ldloc.s    V_0
    IL_08eb:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_08f0:  ret

    IL_08f1:  ldarg.0
    IL_08f2:  ldloc.s    V_2
    IL_08f4:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_08f9:  ldarg.0
    IL_08fa:  ldloc.s    V_4
    IL_08fc:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0901:  ldarg.0
    IL_0902:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::EnsureStorage()
    IL_0907:  ldarg.0
    IL_0908:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_090d:  stloc.s    V_2
    IL_090f:  ldarg.0
    IL_0910:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0915:  stloc.s    V_4
    IL_0917:  ldarg.0
    IL_0918:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_091d:  stloc.s    V_3
    IL_091f:  ldarg.0
    IL_0920:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_0925:  stloc.s    V_5
    IL_0927:  ldloc.s    V_3
    IL_0929:  ldloc.s    V_2
    IL_092b:  dup
    IL_092c:  ldc.i4.1
    IL_092d:  add
    IL_092e:  stloc.s    V_2
    IL_0930:  ldelem.i4
    IL_0931:  switch     ( 
                          IL_0976,
                          IL_0987,
                          IL_0993,
                          IL_09a4,
                          IL_09f4,
                          IL_0a13,
                          IL_0a1f,
                          IL_0a6f,
                          IL_0376,
                          IL_0acc,
                          IL_0afd,
                          IL_0261,
                          IL_0b5a,
                          IL_0baa,
                          IL_0564,
                          IL_0c07)
    IL_0976:  ldloc.s    V_3
    IL_0978:  ldloc.s    V_2
    IL_097a:  dup
    IL_097b:  ldc.i4.1
    IL_097c:  add
    IL_097d:  stloc.s    V_2
    IL_097f:  ldelem.i4
    IL_0980:  stloc.s    V_0
    IL_0982:  br         IL_08e8

    IL_0987:  ldloc.s    V_4
    IL_0989:  ldc.i4.s   1
    IL_098b:  add
    IL_098c:  stloc.s    V_4
    IL_098e:  br         IL_08f1

    IL_0993:  ldloc.s    V_3
    IL_0995:  ldloc.s    V_2
    IL_0997:  dup
    IL_0998:  ldc.i4.1
    IL_0999:  add
    IL_099a:  stloc.s    V_2
    IL_099c:  ldelem.i4
    IL_099d:  stloc.s    V_0
    IL_099f:  br         IL_01c9

    IL_09a4:  ldloc.s    V_3
    IL_09a6:  ldloc.s    V_2
    IL_09a8:  dup
    IL_09a9:  ldc.i4.1
    IL_09aa:  add
    IL_09ab:  stloc.s    V_2
    IL_09ad:  ldelem.i4
    IL_09ae:  stloc.s    V_0
    IL_09b0:  ldloc.s    V_3
    IL_09b2:  ldloc.s    V_2
    IL_09b4:  dup
    IL_09b5:  ldc.i4.1
    IL_09b6:  add
    IL_09b7:  stloc.s    V_2
    IL_09b9:  ldelem.i4
    IL_09ba:  stloc.s    V_6
    IL_09bc:  ldloc.s    V_6
    IL_09be:  ldc.i4.s   0
    IL_09c0:  ble        IL_0194

    IL_09c5:  ldloc.s    V_3
    IL_09c7:  ldloc.s    V_2
    IL_09c9:  ldc.i4.1
    IL_09ca:  sub
    IL_09cb:  dup
    IL_09cc:  stloc.s    V_2
    IL_09ce:  ldloc.s    V_6
    IL_09d0:  ldc.i4.s   1
    IL_09d2:  sub
    IL_09d3:  stelem.i4
    IL_09d4:  ldloc.s    V_3
    IL_09d6:  ldloc.s    V_2
    IL_09d8:  ldc.i4.1
    IL_09d9:  sub
    IL_09da:  dup
    IL_09db:  stloc.s    V_2
    IL_09dd:  ldloc.s    V_0
    IL_09df:  ldc.i4.s   1
    IL_09e1:  sub
    IL_09e2:  stelem.i4
    IL_09e3:  ldloc.s    V_3
    IL_09e5:  ldloc.s    V_2
    IL_09e7:  ldc.i4.1
    IL_09e8:  sub
    IL_09e9:  dup
    IL_09ea:  stloc.s    V_2
    IL_09ec:  ldc.i4.s   3
    IL_09ee:  stelem.i4
    IL_09ef:  br         IL_0194

    IL_09f4:  ldloc.s    V_5
    IL_09f6:  ldloc.s    V_4
    IL_09f8:  ldc.i4.1
    IL_09f9:  sub
    IL_09fa:  dup
    IL_09fb:  stloc.s    V_4
    IL_09fd:  ldloc.s    V_3
    IL_09ff:  ldloc.s    V_2
    IL_0a01:  dup
    IL_0a02:  ldc.i4.1
    IL_0a03:  add
    IL_0a04:  stloc.s    V_2
    IL_0a06:  ldelem.i4
    IL_0a07:  stelem.i4
    IL_0a08:  ldarg.0
    IL_0a09:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Uncapture()
    IL_0a0e:  br         IL_08f1

    IL_0a13:  ldloc.s    V_4
    IL_0a15:  ldc.i4.s   2
    IL_0a17:  add
    IL_0a18:  stloc.s    V_4
    IL_0a1a:  br         IL_08f1

    IL_0a1f:  ldloc.s    V_3
    IL_0a21:  ldloc.s    V_2
    IL_0a23:  dup
    IL_0a24:  ldc.i4.1
    IL_0a25:  add
    IL_0a26:  stloc.s    V_2
    IL_0a28:  ldelem.i4
    IL_0a29:  stloc.s    V_0
    IL_0a2b:  ldloc.s    V_3
    IL_0a2d:  ldloc.s    V_2
    IL_0a2f:  dup
    IL_0a30:  ldc.i4.1
    IL_0a31:  add
    IL_0a32:  stloc.s    V_2
    IL_0a34:  ldelem.i4
    IL_0a35:  stloc.s    V_6
    IL_0a37:  ldloc.s    V_6
    IL_0a39:  ldc.i4.s   0
    IL_0a3b:  ble        IL_034d

    IL_0a40:  ldloc.s    V_3
    IL_0a42:  ldloc.s    V_2
    IL_0a44:  ldc.i4.1
    IL_0a45:  sub
    IL_0a46:  dup
    IL_0a47:  stloc.s    V_2
    IL_0a49:  ldloc.s    V_6
    IL_0a4b:  ldc.i4.s   1
    IL_0a4d:  sub
    IL_0a4e:  stelem.i4
    IL_0a4f:  ldloc.s    V_3
    IL_0a51:  ldloc.s    V_2
    IL_0a53:  ldc.i4.1
    IL_0a54:  sub
    IL_0a55:  dup
    IL_0a56:  stloc.s    V_2
    IL_0a58:  ldloc.s    V_0
    IL_0a5a:  ldc.i4.s   1
    IL_0a5c:  sub
    IL_0a5d:  stelem.i4
    IL_0a5e:  ldloc.s    V_3
    IL_0a60:  ldloc.s    V_2
    IL_0a62:  ldc.i4.1
    IL_0a63:  sub
    IL_0a64:  dup
    IL_0a65:  stloc.s    V_2
    IL_0a67:  ldc.i4.s   6
    IL_0a69:  stelem.i4
    IL_0a6a:  br         IL_034d

    IL_0a6f:  ldloc.s    V_5
    IL_0a71:  ldloc.s    V_4
    IL_0a73:  dup
    IL_0a74:  ldc.i4.1
    IL_0a75:  add
    IL_0a76:  stloc.s    V_4
    IL_0a78:  ldelem.i4
    IL_0a79:  ldc.i4.s   1
    IL_0a7b:  sub
    IL_0a7c:  dup
    IL_0a7d:  stloc.s    V_6
    IL_0a7f:  ldc.i4.s   0
    IL_0a81:  blt.s      IL_0aac

    IL_0a83:  ldloc.s    V_5
    IL_0a85:  ldloc.s    V_4
    IL_0a87:  dup
    IL_0a88:  ldc.i4.1
    IL_0a89:  add
    IL_0a8a:  stloc.s    V_4
    IL_0a8c:  ldelem.i4
    IL_0a8d:  stloc.s    V_0
    IL_0a8f:  ldloc.s    V_3
    IL_0a91:  ldloc.s    V_2
    IL_0a93:  ldc.i4.1
    IL_0a94:  sub
    IL_0a95:  dup
    IL_0a96:  stloc.s    V_2
    IL_0a98:  ldloc.s    V_6
    IL_0a9a:  stelem.i4
    IL_0a9b:  ldloc.s    V_3
    IL_0a9d:  ldloc.s    V_2
    IL_0a9f:  ldc.i4.1
    IL_0aa0:  sub
    IL_0aa1:  dup
    IL_0aa2:  stloc.s    V_2
    IL_0aa4:  ldc.i4.s   9
    IL_0aa6:  stelem.i4
    IL_0aa7:  br         IL_0474

    IL_0aac:  ldloc.s    V_5
    IL_0aae:  ldloc.s    V_4
    IL_0ab0:  ldloc.s    V_3
    IL_0ab2:  ldloc.s    V_2
    IL_0ab4:  dup
    IL_0ab5:  ldc.i4.1
    IL_0ab6:  add
    IL_0ab7:  stloc.s    V_2
    IL_0ab9:  ldelem.i4
    IL_0aba:  stelem.i4
    IL_0abb:  ldloc.s    V_5
    IL_0abd:  ldloc.s    V_4
    IL_0abf:  ldc.i4.1
    IL_0ac0:  sub
    IL_0ac1:  dup
    IL_0ac2:  stloc.s    V_4
    IL_0ac4:  ldloc.s    V_6
    IL_0ac6:  stelem.i4
    IL_0ac7:  br         IL_08f1

    IL_0acc:  ldloc.s    V_3
    IL_0ace:  ldloc.s    V_2
    IL_0ad0:  dup
    IL_0ad1:  ldc.i4.1
    IL_0ad2:  add
    IL_0ad3:  stloc.s    V_2
    IL_0ad5:  ldelem.i4
    IL_0ad6:  stloc.s    V_6
    IL_0ad8:  ldloc.s    V_5
    IL_0ada:  ldloc.s    V_4
    IL_0adc:  ldc.i4.1
    IL_0add:  sub
    IL_0ade:  dup
    IL_0adf:  stloc.s    V_4
    IL_0ae1:  ldloc.s    V_3
    IL_0ae3:  ldloc.s    V_2
    IL_0ae5:  dup
    IL_0ae6:  ldc.i4.1
    IL_0ae7:  add
    IL_0ae8:  stloc.s    V_2
    IL_0aea:  ldelem.i4
    IL_0aeb:  stelem.i4
    IL_0aec:  ldloc.s    V_5
    IL_0aee:  ldloc.s    V_4
    IL_0af0:  ldc.i4.1
    IL_0af1:  sub
    IL_0af2:  dup
    IL_0af3:  stloc.s    V_4
    IL_0af5:  ldloc.s    V_6
    IL_0af7:  stelem.i4
    IL_0af8:  br         IL_08f1

    IL_0afd:  ldloc.s    V_5
    IL_0aff:  ldloc.s    V_4
    IL_0b01:  dup
    IL_0b02:  ldc.i4.1
    IL_0b03:  add
    IL_0b04:  stloc.s    V_4
    IL_0b06:  ldelem.i4
    IL_0b07:  ldc.i4.s   1
    IL_0b09:  sub
    IL_0b0a:  dup
    IL_0b0b:  stloc.s    V_6
    IL_0b0d:  ldc.i4.s   0
    IL_0b0f:  blt.s      IL_0b3a

    IL_0b11:  ldloc.s    V_5
    IL_0b13:  ldloc.s    V_4
    IL_0b15:  dup
    IL_0b16:  ldc.i4.1
    IL_0b17:  add
    IL_0b18:  stloc.s    V_4
    IL_0b1a:  ldelem.i4
    IL_0b1b:  stloc.s    V_0
    IL_0b1d:  ldloc.s    V_3
    IL_0b1f:  ldloc.s    V_2
    IL_0b21:  ldc.i4.1
    IL_0b22:  sub
    IL_0b23:  dup
    IL_0b24:  stloc.s    V_2
    IL_0b26:  ldloc.s    V_6
    IL_0b28:  stelem.i4
    IL_0b29:  ldloc.s    V_3
    IL_0b2b:  ldloc.s    V_2
    IL_0b2d:  ldc.i4.1
    IL_0b2e:  sub
    IL_0b2f:  dup
    IL_0b30:  stloc.s    V_2
    IL_0b32:  ldc.i4.s   9
    IL_0b34:  stelem.i4
    IL_0b35:  br         IL_053b

    IL_0b3a:  ldloc.s    V_5
    IL_0b3c:  ldloc.s    V_4
    IL_0b3e:  ldloc.s    V_3
    IL_0b40:  ldloc.s    V_2
    IL_0b42:  dup
    IL_0b43:  ldc.i4.1
    IL_0b44:  add
    IL_0b45:  stloc.s    V_2
    IL_0b47:  ldelem.i4
    IL_0b48:  stelem.i4
    IL_0b49:  ldloc.s    V_5
    IL_0b4b:  ldloc.s    V_4
    IL_0b4d:  ldc.i4.1
    IL_0b4e:  sub
    IL_0b4f:  dup
    IL_0b50:  stloc.s    V_4
    IL_0b52:  ldloc.s    V_6
    IL_0b54:  stelem.i4
    IL_0b55:  br         IL_08f1

    IL_0b5a:  ldloc.s    V_3
    IL_0b5c:  ldloc.s    V_2
    IL_0b5e:  dup
    IL_0b5f:  ldc.i4.1
    IL_0b60:  add
    IL_0b61:  stloc.s    V_2
    IL_0b63:  ldelem.i4
    IL_0b64:  stloc.s    V_0
    IL_0b66:  ldloc.s    V_3
    IL_0b68:  ldloc.s    V_2
    IL_0b6a:  dup
    IL_0b6b:  ldc.i4.1
    IL_0b6c:  add
    IL_0b6d:  stloc.s    V_2
    IL_0b6f:  ldelem.i4
    IL_0b70:  stloc.s    V_6
    IL_0b72:  ldloc.s    V_6
    IL_0b74:  ldc.i4.s   0
    IL_0b76:  ble        IL_0650

    IL_0b7b:  ldloc.s    V_3
    IL_0b7d:  ldloc.s    V_2
    IL_0b7f:  ldc.i4.1
    IL_0b80:  sub
    IL_0b81:  dup
    IL_0b82:  stloc.s    V_2
    IL_0b84:  ldloc.s    V_6
    IL_0b86:  ldc.i4.s   1
    IL_0b88:  sub
    IL_0b89:  stelem.i4
    IL_0b8a:  ldloc.s    V_3
    IL_0b8c:  ldloc.s    V_2
    IL_0b8e:  ldc.i4.1
    IL_0b8f:  sub
    IL_0b90:  dup
    IL_0b91:  stloc.s    V_2
    IL_0b93:  ldloc.s    V_0
    IL_0b95:  ldc.i4.s   1
    IL_0b97:  sub
    IL_0b98:  stelem.i4
    IL_0b99:  ldloc.s    V_3
    IL_0b9b:  ldloc.s    V_2
    IL_0b9d:  ldc.i4.1
    IL_0b9e:  sub
    IL_0b9f:  dup
    IL_0ba0:  stloc.s    V_2
    IL_0ba2:  ldc.i4.s   12
    IL_0ba4:  stelem.i4
    IL_0ba5:  br         IL_0650

    IL_0baa:  ldloc.s    V_5
    IL_0bac:  ldloc.s    V_4
    IL_0bae:  dup
    IL_0baf:  ldc.i4.1
    IL_0bb0:  add
    IL_0bb1:  stloc.s    V_4
    IL_0bb3:  ldelem.i4
    IL_0bb4:  ldc.i4.s   1
    IL_0bb6:  sub
    IL_0bb7:  dup
    IL_0bb8:  stloc.s    V_6
    IL_0bba:  ldc.i4.s   0
    IL_0bbc:  blt.s      IL_0be7

    IL_0bbe:  ldloc.s    V_5
    IL_0bc0:  ldloc.s    V_4
    IL_0bc2:  dup
    IL_0bc3:  ldc.i4.1
    IL_0bc4:  add
    IL_0bc5:  stloc.s    V_4
    IL_0bc7:  ldelem.i4
    IL_0bc8:  stloc.s    V_0
    IL_0bca:  ldloc.s    V_3
    IL_0bcc:  ldloc.s    V_2
    IL_0bce:  ldc.i4.1
    IL_0bcf:  sub
    IL_0bd0:  dup
    IL_0bd1:  stloc.s    V_2
    IL_0bd3:  ldloc.s    V_6
    IL_0bd5:  stelem.i4
    IL_0bd6:  ldloc.s    V_3
    IL_0bd8:  ldloc.s    V_2
    IL_0bda:  ldc.i4.1
    IL_0bdb:  sub
    IL_0bdc:  dup
    IL_0bdd:  stloc.s    V_2
    IL_0bdf:  ldc.i4.s   9
    IL_0be1:  stelem.i4
    IL_0be2:  br         IL_0736

    IL_0be7:  ldloc.s    V_5
    IL_0be9:  ldloc.s    V_4
    IL_0beb:  ldloc.s    V_3
    IL_0bed:  ldloc.s    V_2
    IL_0bef:  dup
    IL_0bf0:  ldc.i4.1
    IL_0bf1:  add
    IL_0bf2:  stloc.s    V_2
    IL_0bf4:  ldelem.i4
    IL_0bf5:  stelem.i4
    IL_0bf6:  ldloc.s    V_5
    IL_0bf8:  ldloc.s    V_4
    IL_0bfa:  ldc.i4.1
    IL_0bfb:  sub
    IL_0bfc:  dup
    IL_0bfd:  stloc.s    V_4
    IL_0bff:  ldloc.s    V_6
    IL_0c01:  stelem.i4
    IL_0c02:  br         IL_08f1

    IL_0c07:  ldloc.s    V_3
    IL_0c09:  ldloc.s    V_2
    IL_0c0b:  dup
    IL_0c0c:  ldc.i4.1
    IL_0c0d:  add
    IL_0c0e:  stloc.s    V_2
    IL_0c10:  ldelem.i4
    IL_0c11:  stloc.s    V_0
    IL_0c13:  ldloc.s    V_3
    IL_0c15:  ldloc.s    V_2
    IL_0c17:  dup
    IL_0c18:  ldc.i4.1
    IL_0c19:  add
    IL_0c1a:  stloc.s    V_2
    IL_0c1c:  ldelem.i4
    IL_0c1d:  stloc.s    V_6
    IL_0c1f:  ldloc.s    V_6
    IL_0c21:  ldc.i4.s   0
    IL_0c23:  ble        IL_0822

    IL_0c28:  ldloc.s    V_3
    IL_0c2a:  ldloc.s    V_2
    IL_0c2c:  ldc.i4.1
    IL_0c2d:  sub
    IL_0c2e:  dup
    IL_0c2f:  stloc.s    V_2
    IL_0c31:  ldloc.s    V_6
    IL_0c33:  ldc.i4.s   1
    IL_0c35:  sub
    IL_0c36:  stelem.i4
    IL_0c37:  ldloc.s    V_3
    IL_0c39:  ldloc.s    V_2
    IL_0c3b:  ldc.i4.1
    IL_0c3c:  sub
    IL_0c3d:  dup
    IL_0c3e:  stloc.s    V_2
    IL_0c40:  ldloc.s    V_0
    IL_0c42:  ldc.i4.s   1
    IL_0c44:  sub
    IL_0c45:  stelem.i4
    IL_0c46:  ldloc.s    V_3
    IL_0c48:  ldloc.s    V_2
    IL_0c4a:  ldc.i4.1
    IL_0c4b:  sub
    IL_0c4c:  dup
    IL_0c4d:  stloc.s    V_2
    IL_0c4f:  ldc.i4.s   15
    IL_0c51:  stelem.i4
    IL_0c52:  br         IL_0822
  } // end of method BracketedControllerOpenPatternRunner2::Go

  .method public virtual instance bool  FindFirstChar() cil managed
  {
    // Code size       124 (0x7c)
    .maxstack  8
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_000e:  stloc.s    V_3
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0016:  ldc.i4.s   0
    IL_0018:  add
    IL_0019:  stloc.s    V_0
    IL_001b:  br.s       IL_0024

    IL_001d:  ldc.i4.s   1
    IL_001f:  ldloc.s    V_0
    IL_0021:  add
    IL_0022:  stloc.s    V_0
    IL_0024:  ldloc.s    V_0
    IL_0026:  ldloc.s    V_3
    IL_0028:  bge        IL_006d

    IL_002d:  ldloc.s    V_1
    IL_002f:  ldloc.s    V_0
    IL_0031:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0036:  dup
    IL_0037:  stloc.s    V_2
    IL_0039:  ldc.i4.s   36
    IL_003b:  beq        IL_005e

    IL_0040:  ldloc.s    V_2
    IL_0042:  ldc.i4.s   36
    IL_0044:  sub
    IL_0045:  dup
    IL_0046:  stloc.s    V_2
    IL_0048:  ldc.i4.s   0
    IL_004a:  bgt.un.s   IL_001d

    IL_004c:  ldloc.s    V_2
    IL_004e:  switch     ( 
                          IL_0057)
    IL_0057:  ldc.i4.s   0
    IL_0059:  br         IL_001f

    IL_005e:  ldloc.s    V_0
    IL_0060:  stloc.s    V_2
    IL_0062:  ldarg.0
    IL_0063:  ldloc.s    V_2
    IL_0065:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_006a:  ldc.i4.s   1
    IL_006c:  ret

    IL_006d:  ldarg.0
    IL_006e:  ldarg.0
    IL_006f:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_0074:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0079:  ldc.i4.s   0
    IL_007b:  ret
  } // end of method BracketedControllerOpenPatternRunner2::FindFirstChar

  .method public virtual instance void  InitTrackCount() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   32
    IL_0003:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackcount
    IL_0008:  ret
  } // end of method BracketedControllerOpenPatternRunner2::InitTrackCount

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunner::.ctor()
    IL_0006:  ret
  } // end of method BracketedControllerOpenPatternRunner2::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerOpenPatternRunner2

.class private auto ansi Xeora.Web.RegularExpression.BracketedControllerOpenPatternFactory2
       extends [System]System.Text.RegularExpressions.RegexRunnerFactory
{
  .method public virtual instance class [System]System.Text.RegularExpressions.RegexRunner 
          CreateInstance() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  1
    IL_0000:  newobj     instance void Xeora.Web.RegularExpression.BracketedControllerOpenPatternRunner2::.ctor()
    IL_0005:  ret
  } // end of method BracketedControllerOpenPatternFactory2::CreateInstance

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunnerFactory::.ctor()
    IL_0006:  ret
  } // end of method BracketedControllerOpenPatternFactory2::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerOpenPatternFactory2

.class public auto ansi Xeora.Web.RegularExpression.BracketedControllerOpenPattern
       extends [System]System.Text.RegularExpressions.Regex
{
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       334 (0x14e)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.Regex::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldstr      "\\$((\?<ItemID>\\w+)|(\?<DirectiveType>\\w)(\\#\\d+(\\+)\?)"
    + "\?(\\[[\\.\\w\\-]+\\])\?\\:(\?<ItemID>[\\.\\w\\-]+))\\:\\{"
    IL_000c:  stfld      string [System]System.Text.RegularExpressions.Regex::pattern
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.s   2
    IL_0014:  stfld      valuetype [System]System.Text.RegularExpressions.RegexOptions [System]System.Text.RegularExpressions.Regex::roptions
    IL_0019:  ldarg.0
    IL_001a:  newobj     instance void Xeora.Web.RegularExpression.BracketedControllerOpenPatternFactory2::.ctor()
    IL_001f:  stfld      class [System]System.Text.RegularExpressions.RegexRunnerFactory [System]System.Text.RegularExpressions.Regex::factory
    IL_0024:  ldarg.0
    IL_0025:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor()
    IL_002a:  stfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_002f:  ldarg.0
    IL_0030:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_0035:  ldstr      "3"
    IL_003a:  ldc.i4.s   3
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0046:  ldarg.0
    IL_0047:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_004c:  ldstr      "0"
    IL_0051:  ldc.i4.s   0
    IL_0053:  box        [mscorlib]System.Int32
    IL_0058:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_005d:  ldarg.0
    IL_005e:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_0063:  ldstr      "1"
    IL_0068:  ldc.i4.s   1
    IL_006a:  box        [mscorlib]System.Int32
    IL_006f:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0074:  ldarg.0
    IL_0075:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_007a:  ldstr      "4"
    IL_007f:  ldc.i4.s   4
    IL_0081:  box        [mscorlib]System.Int32
    IL_0086:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_008b:  ldarg.0
    IL_008c:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_0091:  ldstr      "DirectiveType"
    IL_0096:  ldc.i4.s   6
    IL_0098:  box        [mscorlib]System.Int32
    IL_009d:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00a2:  ldarg.0
    IL_00a3:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_00a8:  ldstr      "ItemID"
    IL_00ad:  ldc.i4.s   5
    IL_00af:  box        [mscorlib]System.Int32
    IL_00b4:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00b9:  ldarg.0
    IL_00ba:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_00bf:  ldstr      "2"
    IL_00c4:  ldc.i4.s   2
    IL_00c6:  box        [mscorlib]System.Int32
    IL_00cb:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_00d0:  ldarg.0
    IL_00d1:  ldc.i4.s   7
    IL_00d3:  newarr     [mscorlib]System.String
    IL_00d8:  stfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_00dd:  ldarg.0
    IL_00de:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_00e3:  ldc.i4.s   0
    IL_00e5:  ldstr      "0"
    IL_00ea:  stelem.ref
    IL_00eb:  ldarg.0
    IL_00ec:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_00f1:  ldc.i4.s   1
    IL_00f3:  ldstr      "1"
    IL_00f8:  stelem.ref
    IL_00f9:  ldarg.0
    IL_00fa:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_00ff:  ldc.i4.s   2
    IL_0101:  ldstr      "2"
    IL_0106:  stelem.ref
    IL_0107:  ldarg.0
    IL_0108:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_010d:  ldc.i4.s   3
    IL_010f:  ldstr      "3"
    IL_0114:  stelem.ref
    IL_0115:  ldarg.0
    IL_0116:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_011b:  ldc.i4.s   4
    IL_011d:  ldstr      "4"
    IL_0122:  stelem.ref
    IL_0123:  ldarg.0
    IL_0124:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_0129:  ldc.i4.s   5
    IL_012b:  ldstr      "ItemID"
    IL_0130:  stelem.ref
    IL_0131:  ldarg.0
    IL_0132:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_0137:  ldc.i4.s   6
    IL_0139:  ldstr      "DirectiveType"
    IL_013e:  stelem.ref
    IL_013f:  ldarg.0
    IL_0140:  ldc.i4.s   7
    IL_0142:  stfld      int32 [System]System.Text.RegularExpressions.Regex::capsize
    IL_0147:  ldarg.0
    IL_0148:  call       instance void [System]System.Text.RegularExpressions.Regex::InitializeReferences()
    IL_014d:  ret
  } // end of method BracketedControllerOpenPattern::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerOpenPattern

.class private auto ansi Xeora.Web.RegularExpression.BracketedControllerSeparatorPatternRunner3
       extends [System]System.Text.RegularExpressions.RegexRunner
{
  .method public virtual instance void  Go() cil managed
  {
    // Code size       763 (0x2fb)
    .maxstack  21
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32[] V_3,
             int32 V_4,
             int32[] V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10,
             int32 V_11)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextstart
    IL_000e:  stloc.s    V_11
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextbeg
    IL_0016:  stloc.s    V_9
    IL_0018:  ldarg.0
    IL_0019:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_001e:  stloc.s    V_10
    IL_0020:  ldarg.0
    IL_0021:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0026:  stloc.s    V_0
    IL_0028:  ldarg.0
    IL_0029:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_002e:  stloc.s    V_3
    IL_0030:  ldarg.0
    IL_0031:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_0036:  stloc.s    V_2
    IL_0038:  ldarg.0
    IL_0039:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_003e:  stloc.s    V_5
    IL_0040:  ldarg.0
    IL_0041:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0046:  stloc.s    V_4
    IL_0048:  ldloc.s    V_3
    IL_004a:  ldloc.s    V_2
    IL_004c:  ldc.i4.1
    IL_004d:  sub
    IL_004e:  dup
    IL_004f:  stloc.s    V_2
    IL_0051:  ldloc.s    V_0
    IL_0053:  stelem.i4
    IL_0054:  ldloc.s    V_3
    IL_0056:  ldloc.s    V_2
    IL_0058:  ldc.i4.1
    IL_0059:  sub
    IL_005a:  dup
    IL_005b:  stloc.s    V_2
    IL_005d:  ldc.i4.s   0
    IL_005f:  stelem.i4
    IL_0060:  ldloc.s    V_5
    IL_0062:  ldloc.s    V_4
    IL_0064:  ldc.i4.1
    IL_0065:  sub
    IL_0066:  dup
    IL_0067:  stloc.s    V_4
    IL_0069:  ldloc.s    V_0
    IL_006b:  stelem.i4
    IL_006c:  ldloc.s    V_3
    IL_006e:  ldloc.s    V_2
    IL_0070:  ldc.i4.1
    IL_0071:  sub
    IL_0072:  dup
    IL_0073:  stloc.s    V_2
    IL_0075:  ldc.i4.s   1
    IL_0077:  stelem.i4
    IL_0078:  ldc.i4.s   2
    IL_007a:  ldloc.s    V_10
    IL_007c:  ldloc.s    V_0
    IL_007e:  sub
    IL_007f:  bgt        IL_021a

    IL_0084:  ldloc.s    V_1
    IL_0086:  ldloc.s    V_0
    IL_0088:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_008d:  ldc.i4.s   125
    IL_008f:  bne.un     IL_021a

    IL_0094:  ldloc.s    V_1
    IL_0096:  ldloc.s    V_0
    IL_0098:  ldc.i4.s   1
    IL_009a:  add
    IL_009b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00a0:  ldc.i4.s   58
    IL_00a2:  bne.un     IL_021a

    IL_00a7:  ldloc.s    V_0
    IL_00a9:  ldc.i4.s   2
    IL_00ab:  add
    IL_00ac:  stloc.s    V_0
    IL_00ae:  ldloc.s    V_5
    IL_00b0:  ldloc.s    V_4
    IL_00b2:  ldc.i4.1
    IL_00b3:  sub
    IL_00b4:  dup
    IL_00b5:  stloc.s    V_4
    IL_00b7:  ldloc.s    V_0
    IL_00b9:  stelem.i4
    IL_00ba:  ldloc.s    V_3
    IL_00bc:  ldloc.s    V_2
    IL_00be:  ldc.i4.1
    IL_00bf:  sub
    IL_00c0:  dup
    IL_00c1:  stloc.s    V_2
    IL_00c3:  ldc.i4.s   1
    IL_00c5:  stelem.i4
    IL_00c6:  ldc.i4.s   1
    IL_00c8:  ldloc.s    V_10
    IL_00ca:  ldloc.s    V_0
    IL_00cc:  sub
    IL_00cd:  bgt        IL_021a

    IL_00d2:  ldloc.s    V_0
    IL_00d4:  ldc.i4.s   1
    IL_00d6:  add
    IL_00d7:  stloc.s    V_0
    IL_00d9:  ldc.i4.s   1
    IL_00db:  stloc.s    V_6
    IL_00dd:  ldloc.s    V_1
    IL_00df:  ldloc.s    V_0
    IL_00e1:  ldloc.s    V_6
    IL_00e3:  dup
    IL_00e4:  ldc.i4.s   1
    IL_00e6:  sub
    IL_00e7:  stloc.s    V_6
    IL_00e9:  sub
    IL_00ea:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00ef:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_00f4:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_00f9:  brfalse    IL_021a

    IL_00fe:  ldloc.s    V_6
    IL_0100:  ldc.i4.s   0
    IL_0102:  bgt        IL_00dd

    IL_0107:  ldloc.s    V_10
    IL_0109:  ldloc.s    V_0
    IL_010b:  sub
    IL_010c:  dup
    IL_010d:  stloc.s    V_7
    IL_010f:  ldc.i4.s   1
    IL_0111:  add
    IL_0112:  stloc.s    V_6
    IL_0114:  ldloc.s    V_6
    IL_0116:  ldc.i4.s   1
    IL_0118:  sub
    IL_0119:  dup
    IL_011a:  stloc.s    V_6
    IL_011c:  ldc.i4.s   0
    IL_011e:  ble        IL_0148

    IL_0123:  ldloc.s    V_1
    IL_0125:  ldloc.s    V_0
    IL_0127:  dup
    IL_0128:  ldc.i4.s   1
    IL_012a:  add
    IL_012b:  stloc.s    V_0
    IL_012d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0132:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0137:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_013c:  brtrue     IL_0114

    IL_0141:  ldloc.s    V_0
    IL_0143:  ldc.i4.s   1
    IL_0145:  sub
    IL_0146:  stloc.s    V_0
    IL_0148:  ldloc.s    V_7
    IL_014a:  ldloc.s    V_6
    IL_014c:  ble.s      IL_017b

    IL_014e:  ldloc.s    V_3
    IL_0150:  ldloc.s    V_2
    IL_0152:  ldc.i4.1
    IL_0153:  sub
    IL_0154:  dup
    IL_0155:  stloc.s    V_2
    IL_0157:  ldloc.s    V_7
    IL_0159:  ldloc.s    V_6
    IL_015b:  sub
    IL_015c:  ldc.i4.s   1
    IL_015e:  sub
    IL_015f:  stelem.i4
    IL_0160:  ldloc.s    V_3
    IL_0162:  ldloc.s    V_2
    IL_0164:  ldc.i4.1
    IL_0165:  sub
    IL_0166:  dup
    IL_0167:  stloc.s    V_2
    IL_0169:  ldloc.s    V_0
    IL_016b:  ldc.i4.s   1
    IL_016d:  sub
    IL_016e:  stelem.i4
    IL_016f:  ldloc.s    V_3
    IL_0171:  ldloc.s    V_2
    IL_0173:  ldc.i4.1
    IL_0174:  sub
    IL_0175:  dup
    IL_0176:  stloc.s    V_2
    IL_0178:  ldc.i4.s   2
    IL_017a:  stelem.i4
    IL_017b:  ldloc.s    V_5
    IL_017d:  ldloc.s    V_4
    IL_017f:  dup
    IL_0180:  ldc.i4.1
    IL_0181:  add
    IL_0182:  stloc.s    V_4
    IL_0184:  ldelem.i4
    IL_0185:  stloc.s    V_6
    IL_0187:  ldarg.0
    IL_0188:  ldc.i4.s   1
    IL_018a:  ldloc.s    V_6
    IL_018c:  ldloc.s    V_0
    IL_018e:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0193:  ldloc.s    V_3
    IL_0195:  ldloc.s    V_2
    IL_0197:  ldc.i4.1
    IL_0198:  sub
    IL_0199:  dup
    IL_019a:  stloc.s    V_2
    IL_019c:  ldloc.s    V_6
    IL_019e:  stelem.i4
    IL_019f:  ldloc.s    V_3
    IL_01a1:  ldloc.s    V_2
    IL_01a3:  ldc.i4.1
    IL_01a4:  sub
    IL_01a5:  dup
    IL_01a6:  stloc.s    V_2
    IL_01a8:  ldc.i4.s   3
    IL_01aa:  stelem.i4
    IL_01ab:  ldc.i4.s   2
    IL_01ad:  ldloc.s    V_10
    IL_01af:  ldloc.s    V_0
    IL_01b1:  sub
    IL_01b2:  bgt        IL_021a

    IL_01b7:  ldloc.s    V_1
    IL_01b9:  ldloc.s    V_0
    IL_01bb:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_01c0:  ldc.i4.s   58
    IL_01c2:  bne.un     IL_021a

    IL_01c7:  ldloc.s    V_1
    IL_01c9:  ldloc.s    V_0
    IL_01cb:  ldc.i4.s   1
    IL_01cd:  add
    IL_01ce:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_01d3:  ldc.i4.s   123
    IL_01d5:  bne.un     IL_021a

    IL_01da:  ldloc.s    V_0
    IL_01dc:  ldc.i4.s   2
    IL_01de:  add
    IL_01df:  stloc.s    V_0
    IL_01e1:  ldloc.s    V_5
    IL_01e3:  ldloc.s    V_4
    IL_01e5:  dup
    IL_01e6:  ldc.i4.1
    IL_01e7:  add
    IL_01e8:  stloc.s    V_4
    IL_01ea:  ldelem.i4
    IL_01eb:  stloc.s    V_6
    IL_01ed:  ldarg.0
    IL_01ee:  ldc.i4.s   0
    IL_01f0:  ldloc.s    V_6
    IL_01f2:  ldloc.s    V_0
    IL_01f4:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_01f9:  ldloc.s    V_3
    IL_01fb:  ldloc.s    V_2
    IL_01fd:  ldc.i4.1
    IL_01fe:  sub
    IL_01ff:  dup
    IL_0200:  stloc.s    V_2
    IL_0202:  ldloc.s    V_6
    IL_0204:  stelem.i4
    IL_0205:  ldloc.s    V_3
    IL_0207:  ldloc.s    V_2
    IL_0209:  ldc.i4.1
    IL_020a:  sub
    IL_020b:  dup
    IL_020c:  stloc.s    V_2
    IL_020e:  ldc.i4.s   3
    IL_0210:  stelem.i4
    IL_0211:  ldarg.0
    IL_0212:  ldloc.s    V_0
    IL_0214:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0219:  ret

    IL_021a:  ldarg.0
    IL_021b:  ldloc.s    V_2
    IL_021d:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_0222:  ldarg.0
    IL_0223:  ldloc.s    V_4
    IL_0225:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_022a:  ldarg.0
    IL_022b:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::EnsureStorage()
    IL_0230:  ldarg.0
    IL_0231:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_0236:  stloc.s    V_2
    IL_0238:  ldarg.0
    IL_0239:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_023e:  stloc.s    V_4
    IL_0240:  ldarg.0
    IL_0241:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_0246:  stloc.s    V_3
    IL_0248:  ldarg.0
    IL_0249:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_024e:  stloc.s    V_5
    IL_0250:  ldloc.s    V_3
    IL_0252:  ldloc.s    V_2
    IL_0254:  dup
    IL_0255:  ldc.i4.1
    IL_0256:  add
    IL_0257:  stloc.s    V_2
    IL_0259:  ldelem.i4
    IL_025a:  switch     ( 
                          IL_026f,
                          IL_0280,
                          IL_028c,
                          IL_02dc)
    IL_026f:  ldloc.s    V_3
    IL_0271:  ldloc.s    V_2
    IL_0273:  dup
    IL_0274:  ldc.i4.1
    IL_0275:  add
    IL_0276:  stloc.s    V_2
    IL_0278:  ldelem.i4
    IL_0279:  stloc.s    V_0
    IL_027b:  br         IL_0211

    IL_0280:  ldloc.s    V_4
    IL_0282:  ldc.i4.s   1
    IL_0284:  add
    IL_0285:  stloc.s    V_4
    IL_0287:  br         IL_021a

    IL_028c:  ldloc.s    V_3
    IL_028e:  ldloc.s    V_2
    IL_0290:  dup
    IL_0291:  ldc.i4.1
    IL_0292:  add
    IL_0293:  stloc.s    V_2
    IL_0295:  ldelem.i4
    IL_0296:  stloc.s    V_0
    IL_0298:  ldloc.s    V_3
    IL_029a:  ldloc.s    V_2
    IL_029c:  dup
    IL_029d:  ldc.i4.1
    IL_029e:  add
    IL_029f:  stloc.s    V_2
    IL_02a1:  ldelem.i4
    IL_02a2:  stloc.s    V_6
    IL_02a4:  ldloc.s    V_6
    IL_02a6:  ldc.i4.s   0
    IL_02a8:  ble        IL_017b

    IL_02ad:  ldloc.s    V_3
    IL_02af:  ldloc.s    V_2
    IL_02b1:  ldc.i4.1
    IL_02b2:  sub
    IL_02b3:  dup
    IL_02b4:  stloc.s    V_2
    IL_02b6:  ldloc.s    V_6
    IL_02b8:  ldc.i4.s   1
    IL_02ba:  sub
    IL_02bb:  stelem.i4
    IL_02bc:  ldloc.s    V_3
    IL_02be:  ldloc.s    V_2
    IL_02c0:  ldc.i4.1
    IL_02c1:  sub
    IL_02c2:  dup
    IL_02c3:  stloc.s    V_2
    IL_02c5:  ldloc.s    V_0
    IL_02c7:  ldc.i4.s   1
    IL_02c9:  sub
    IL_02ca:  stelem.i4
    IL_02cb:  ldloc.s    V_3
    IL_02cd:  ldloc.s    V_2
    IL_02cf:  ldc.i4.1
    IL_02d0:  sub
    IL_02d1:  dup
    IL_02d2:  stloc.s    V_2
    IL_02d4:  ldc.i4.s   2
    IL_02d6:  stelem.i4
    IL_02d7:  br         IL_017b

    IL_02dc:  ldloc.s    V_5
    IL_02de:  ldloc.s    V_4
    IL_02e0:  ldc.i4.1
    IL_02e1:  sub
    IL_02e2:  dup
    IL_02e3:  stloc.s    V_4
    IL_02e5:  ldloc.s    V_3
    IL_02e7:  ldloc.s    V_2
    IL_02e9:  dup
    IL_02ea:  ldc.i4.1
    IL_02eb:  add
    IL_02ec:  stloc.s    V_2
    IL_02ee:  ldelem.i4
    IL_02ef:  stelem.i4
    IL_02f0:  ldarg.0
    IL_02f1:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Uncapture()
    IL_02f6:  br         IL_021a
  } // end of method BracketedControllerSeparatorPatternRunner3::Go

  .method public virtual instance bool  FindFirstChar() cil managed
  {
    // Code size       887 (0x377)
    .maxstack  78
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_000e:  stloc.s    V_3
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0016:  ldc.i4.s   1
    IL_0018:  add
    IL_0019:  stloc.s    V_0
    IL_001b:  br.s       IL_0024

    IL_001d:  ldc.i4.s   2
    IL_001f:  ldloc.s    V_0
    IL_0021:  add
    IL_0022:  stloc.s    V_0
    IL_0024:  ldloc.s    V_0
    IL_0026:  ldloc.s    V_3
    IL_0028:  bge        IL_0368

    IL_002d:  ldloc.s    V_1
    IL_002f:  ldloc.s    V_0
    IL_0031:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0036:  dup
    IL_0037:  stloc.s    V_2
    IL_0039:  ldc.i4.s   58
    IL_003b:  beq        IL_033f

    IL_0040:  ldloc.s    V_2
    IL_0042:  ldc.i4.s   58
    IL_0044:  sub
    IL_0045:  dup
    IL_0046:  stloc.s    V_2
    IL_0048:  ldc.i4.s   67
    IL_004a:  bgt.un.s   IL_001d

    IL_004c:  ldloc.s    V_2
    IL_004e:  switch     ( 
                          IL_0163,
                          IL_016a,
                          IL_0171,
                          IL_0178,
                          IL_017f,
                          IL_0186,
                          IL_018d,
                          IL_0194,
                          IL_019b,
                          IL_01a2,
                          IL_01a9,
                          IL_01b0,
                          IL_01b7,
                          IL_01be,
                          IL_01c5,
                          IL_01cc,
                          IL_01d3,
                          IL_01da,
                          IL_01e1,
                          IL_01e8,
                          IL_01ef,
                          IL_01f6,
                          IL_01fd,
                          IL_0204,
                          IL_020b,
                          IL_0212,
                          IL_0219,
                          IL_0220,
                          IL_0227,
                          IL_022e,
                          IL_0235,
                          IL_023c,
                          IL_0243,
                          IL_024a,
                          IL_0251,
                          IL_0258,
                          IL_025f,
                          IL_0266,
                          IL_026d,
                          IL_0274,
                          IL_027b,
                          IL_0282,
                          IL_0289,
                          IL_0290,
                          IL_0297,
                          IL_029e,
                          IL_02a5,
                          IL_02ac,
                          IL_02b3,
                          IL_02ba,
                          IL_02c1,
                          IL_02c8,
                          IL_02cf,
                          IL_02d6,
                          IL_02dd,
                          IL_02e4,
                          IL_02eb,
                          IL_02f2,
                          IL_02f9,
                          IL_0300,
                          IL_0307,
                          IL_030e,
                          IL_0315,
                          IL_031c,
                          IL_0323,
                          IL_032a,
                          IL_0331,
                          IL_0338)
    IL_0163:  ldc.i4.s   0
    IL_0165:  br         IL_001f

    IL_016a:  ldc.i4.s   2
    IL_016c:  br         IL_001f

    IL_0171:  ldc.i4.s   2
    IL_0173:  br         IL_001f

    IL_0178:  ldc.i4.s   2
    IL_017a:  br         IL_001f

    IL_017f:  ldc.i4.s   2
    IL_0181:  br         IL_001f

    IL_0186:  ldc.i4.s   2
    IL_0188:  br         IL_001f

    IL_018d:  ldc.i4.s   2
    IL_018f:  br         IL_001f

    IL_0194:  ldc.i4.s   2
    IL_0196:  br         IL_001f

    IL_019b:  ldc.i4.s   2
    IL_019d:  br         IL_001f

    IL_01a2:  ldc.i4.s   2
    IL_01a4:  br         IL_001f

    IL_01a9:  ldc.i4.s   2
    IL_01ab:  br         IL_001f

    IL_01b0:  ldc.i4.s   2
    IL_01b2:  br         IL_001f

    IL_01b7:  ldc.i4.s   2
    IL_01b9:  br         IL_001f

    IL_01be:  ldc.i4.s   2
    IL_01c0:  br         IL_001f

    IL_01c5:  ldc.i4.s   2
    IL_01c7:  br         IL_001f

    IL_01cc:  ldc.i4.s   2
    IL_01ce:  br         IL_001f

    IL_01d3:  ldc.i4.s   2
    IL_01d5:  br         IL_001f

    IL_01da:  ldc.i4.s   2
    IL_01dc:  br         IL_001f

    IL_01e1:  ldc.i4.s   2
    IL_01e3:  br         IL_001f

    IL_01e8:  ldc.i4.s   2
    IL_01ea:  br         IL_001f

    IL_01ef:  ldc.i4.s   2
    IL_01f1:  br         IL_001f

    IL_01f6:  ldc.i4.s   2
    IL_01f8:  br         IL_001f

    IL_01fd:  ldc.i4.s   2
    IL_01ff:  br         IL_001f

    IL_0204:  ldc.i4.s   2
    IL_0206:  br         IL_001f

    IL_020b:  ldc.i4.s   2
    IL_020d:  br         IL_001f

    IL_0212:  ldc.i4.s   2
    IL_0214:  br         IL_001f

    IL_0219:  ldc.i4.s   2
    IL_021b:  br         IL_001f

    IL_0220:  ldc.i4.s   2
    IL_0222:  br         IL_001f

    IL_0227:  ldc.i4.s   2
    IL_0229:  br         IL_001f

    IL_022e:  ldc.i4.s   2
    IL_0230:  br         IL_001f

    IL_0235:  ldc.i4.s   2
    IL_0237:  br         IL_001f

    IL_023c:  ldc.i4.s   2
    IL_023e:  br         IL_001f

    IL_0243:  ldc.i4.s   2
    IL_0245:  br         IL_001f

    IL_024a:  ldc.i4.s   2
    IL_024c:  br         IL_001f

    IL_0251:  ldc.i4.s   2
    IL_0253:  br         IL_001f

    IL_0258:  ldc.i4.s   2
    IL_025a:  br         IL_001f

    IL_025f:  ldc.i4.s   2
    IL_0261:  br         IL_001f

    IL_0266:  ldc.i4.s   2
    IL_0268:  br         IL_001f

    IL_026d:  ldc.i4.s   2
    IL_026f:  br         IL_001f

    IL_0274:  ldc.i4.s   2
    IL_0276:  br         IL_001f

    IL_027b:  ldc.i4.s   2
    IL_027d:  br         IL_001f

    IL_0282:  ldc.i4.s   2
    IL_0284:  br         IL_001f

    IL_0289:  ldc.i4.s   2
    IL_028b:  br         IL_001f

    IL_0290:  ldc.i4.s   2
    IL_0292:  br         IL_001f

    IL_0297:  ldc.i4.s   2
    IL_0299:  br         IL_001f

    IL_029e:  ldc.i4.s   2
    IL_02a0:  br         IL_001f

    IL_02a5:  ldc.i4.s   2
    IL_02a7:  br         IL_001f

    IL_02ac:  ldc.i4.s   2
    IL_02ae:  br         IL_001f

    IL_02b3:  ldc.i4.s   2
    IL_02b5:  br         IL_001f

    IL_02ba:  ldc.i4.s   2
    IL_02bc:  br         IL_001f

    IL_02c1:  ldc.i4.s   2
    IL_02c3:  br         IL_001f

    IL_02c8:  ldc.i4.s   2
    IL_02ca:  br         IL_001f

    IL_02cf:  ldc.i4.s   2
    IL_02d1:  br         IL_001f

    IL_02d6:  ldc.i4.s   2
    IL_02d8:  br         IL_001f

    IL_02dd:  ldc.i4.s   2
    IL_02df:  br         IL_001f

    IL_02e4:  ldc.i4.s   2
    IL_02e6:  br         IL_001f

    IL_02eb:  ldc.i4.s   2
    IL_02ed:  br         IL_001f

    IL_02f2:  ldc.i4.s   2
    IL_02f4:  br         IL_001f

    IL_02f9:  ldc.i4.s   2
    IL_02fb:  br         IL_001f

    IL_0300:  ldc.i4.s   2
    IL_0302:  br         IL_001f

    IL_0307:  ldc.i4.s   2
    IL_0309:  br         IL_001f

    IL_030e:  ldc.i4.s   2
    IL_0310:  br         IL_001f

    IL_0315:  ldc.i4.s   2
    IL_0317:  br         IL_001f

    IL_031c:  ldc.i4.s   2
    IL_031e:  br         IL_001f

    IL_0323:  ldc.i4.s   2
    IL_0325:  br         IL_001f

    IL_032a:  ldc.i4.s   2
    IL_032c:  br         IL_001f

    IL_0331:  ldc.i4.s   2
    IL_0333:  br         IL_001f

    IL_0338:  ldc.i4.s   1
    IL_033a:  br         IL_001f

    IL_033f:  ldloc.s    V_0
    IL_0341:  stloc.s    V_2
    IL_0343:  ldloc.s    V_1
    IL_0345:  ldloc.s    V_2
    IL_0347:  ldc.i4.s   1
    IL_0349:  sub
    IL_034a:  dup
    IL_034b:  stloc.s    V_2
    IL_034d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0352:  ldc.i4.s   125
    IL_0354:  beq.s      IL_035d

    IL_0356:  ldc.i4.s   1
    IL_0358:  br         IL_001f

    IL_035d:  ldarg.0
    IL_035e:  ldloc.s    V_2
    IL_0360:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0365:  ldc.i4.s   1
    IL_0367:  ret

    IL_0368:  ldarg.0
    IL_0369:  ldarg.0
    IL_036a:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_036f:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0374:  ldc.i4.s   0
    IL_0376:  ret
  } // end of method BracketedControllerSeparatorPatternRunner3::FindFirstChar

  .method public virtual instance void  InitTrackCount() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   6
    IL_0003:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackcount
    IL_0008:  ret
  } // end of method BracketedControllerSeparatorPatternRunner3::InitTrackCount

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunner::.ctor()
    IL_0006:  ret
  } // end of method BracketedControllerSeparatorPatternRunner3::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerSeparatorPatternRunner3

.class private auto ansi Xeora.Web.RegularExpression.BracketedControllerSeparatorPatternFactory3
       extends [System]System.Text.RegularExpressions.RegexRunnerFactory
{
  .method public virtual instance class [System]System.Text.RegularExpressions.RegexRunner 
          CreateInstance() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  1
    IL_0000:  newobj     instance void Xeora.Web.RegularExpression.BracketedControllerSeparatorPatternRunner3::.ctor()
    IL_0005:  ret
  } // end of method BracketedControllerSeparatorPatternFactory3::CreateInstance

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunnerFactory::.ctor()
    IL_0006:  ret
  } // end of method BracketedControllerSeparatorPatternFactory3::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerSeparatorPatternFactory3

.class public auto ansi Xeora.Web.RegularExpression.BracketedControllerSeparatorPattern
       extends [System]System.Text.RegularExpressions.Regex
{
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       149 (0x95)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.Regex::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldstr      "\\}:(\?<ItemID>[\\.\\w\\-]+)\\:\\{"
    IL_000c:  stfld      string [System]System.Text.RegularExpressions.Regex::pattern
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.s   2
    IL_0014:  stfld      valuetype [System]System.Text.RegularExpressions.RegexOptions [System]System.Text.RegularExpressions.Regex::roptions
    IL_0019:  ldarg.0
    IL_001a:  newobj     instance void Xeora.Web.RegularExpression.BracketedControllerSeparatorPatternFactory3::.ctor()
    IL_001f:  stfld      class [System]System.Text.RegularExpressions.RegexRunnerFactory [System]System.Text.RegularExpressions.Regex::factory
    IL_0024:  ldarg.0
    IL_0025:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor()
    IL_002a:  stfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_002f:  ldarg.0
    IL_0030:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_0035:  ldstr      "0"
    IL_003a:  ldc.i4.s   0
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0046:  ldarg.0
    IL_0047:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_004c:  ldstr      "ItemID"
    IL_0051:  ldc.i4.s   1
    IL_0053:  box        [mscorlib]System.Int32
    IL_0058:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_005d:  ldarg.0
    IL_005e:  ldc.i4.s   2
    IL_0060:  newarr     [mscorlib]System.String
    IL_0065:  stfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_006a:  ldarg.0
    IL_006b:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_0070:  ldc.i4.s   0
    IL_0072:  ldstr      "0"
    IL_0077:  stelem.ref
    IL_0078:  ldarg.0
    IL_0079:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_007e:  ldc.i4.s   1
    IL_0080:  ldstr      "ItemID"
    IL_0085:  stelem.ref
    IL_0086:  ldarg.0
    IL_0087:  ldc.i4.s   2
    IL_0089:  stfld      int32 [System]System.Text.RegularExpressions.Regex::capsize
    IL_008e:  ldarg.0
    IL_008f:  call       instance void [System]System.Text.RegularExpressions.Regex::InitializeReferences()
    IL_0094:  ret
  } // end of method BracketedControllerSeparatorPattern::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerSeparatorPattern

.class private auto ansi Xeora.Web.RegularExpression.BracketedControllerClosePatternRunner4
       extends [System]System.Text.RegularExpressions.RegexRunner
{
  .method public virtual instance void  Go() cil managed
  {
    // Code size       740 (0x2e4)
    .maxstack  21
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32[] V_3,
             int32 V_4,
             int32[] V_5,
             int32 V_6,
             int32 V_7,
             int32 V_8,
             int32 V_9,
             int32 V_10,
             int32 V_11)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextstart
    IL_000e:  stloc.s    V_11
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextbeg
    IL_0016:  stloc.s    V_9
    IL_0018:  ldarg.0
    IL_0019:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_001e:  stloc.s    V_10
    IL_0020:  ldarg.0
    IL_0021:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0026:  stloc.s    V_0
    IL_0028:  ldarg.0
    IL_0029:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_002e:  stloc.s    V_3
    IL_0030:  ldarg.0
    IL_0031:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_0036:  stloc.s    V_2
    IL_0038:  ldarg.0
    IL_0039:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_003e:  stloc.s    V_5
    IL_0040:  ldarg.0
    IL_0041:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0046:  stloc.s    V_4
    IL_0048:  ldloc.s    V_3
    IL_004a:  ldloc.s    V_2
    IL_004c:  ldc.i4.1
    IL_004d:  sub
    IL_004e:  dup
    IL_004f:  stloc.s    V_2
    IL_0051:  ldloc.s    V_0
    IL_0053:  stelem.i4
    IL_0054:  ldloc.s    V_3
    IL_0056:  ldloc.s    V_2
    IL_0058:  ldc.i4.1
    IL_0059:  sub
    IL_005a:  dup
    IL_005b:  stloc.s    V_2
    IL_005d:  ldc.i4.s   0
    IL_005f:  stelem.i4
    IL_0060:  ldloc.s    V_5
    IL_0062:  ldloc.s    V_4
    IL_0064:  ldc.i4.1
    IL_0065:  sub
    IL_0066:  dup
    IL_0067:  stloc.s    V_4
    IL_0069:  ldloc.s    V_0
    IL_006b:  stelem.i4
    IL_006c:  ldloc.s    V_3
    IL_006e:  ldloc.s    V_2
    IL_0070:  ldc.i4.1
    IL_0071:  sub
    IL_0072:  dup
    IL_0073:  stloc.s    V_2
    IL_0075:  ldc.i4.s   1
    IL_0077:  stelem.i4
    IL_0078:  ldc.i4.s   2
    IL_007a:  ldloc.s    V_10
    IL_007c:  ldloc.s    V_0
    IL_007e:  sub
    IL_007f:  bgt        IL_0203

    IL_0084:  ldloc.s    V_1
    IL_0086:  ldloc.s    V_0
    IL_0088:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_008d:  ldc.i4.s   125
    IL_008f:  bne.un     IL_0203

    IL_0094:  ldloc.s    V_1
    IL_0096:  ldloc.s    V_0
    IL_0098:  ldc.i4.s   1
    IL_009a:  add
    IL_009b:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00a0:  ldc.i4.s   58
    IL_00a2:  bne.un     IL_0203

    IL_00a7:  ldloc.s    V_0
    IL_00a9:  ldc.i4.s   2
    IL_00ab:  add
    IL_00ac:  stloc.s    V_0
    IL_00ae:  ldloc.s    V_5
    IL_00b0:  ldloc.s    V_4
    IL_00b2:  ldc.i4.1
    IL_00b3:  sub
    IL_00b4:  dup
    IL_00b5:  stloc.s    V_4
    IL_00b7:  ldloc.s    V_0
    IL_00b9:  stelem.i4
    IL_00ba:  ldloc.s    V_3
    IL_00bc:  ldloc.s    V_2
    IL_00be:  ldc.i4.1
    IL_00bf:  sub
    IL_00c0:  dup
    IL_00c1:  stloc.s    V_2
    IL_00c3:  ldc.i4.s   1
    IL_00c5:  stelem.i4
    IL_00c6:  ldc.i4.s   1
    IL_00c8:  ldloc.s    V_10
    IL_00ca:  ldloc.s    V_0
    IL_00cc:  sub
    IL_00cd:  bgt        IL_0203

    IL_00d2:  ldloc.s    V_0
    IL_00d4:  ldc.i4.s   1
    IL_00d6:  add
    IL_00d7:  stloc.s    V_0
    IL_00d9:  ldc.i4.s   1
    IL_00db:  stloc.s    V_6
    IL_00dd:  ldloc.s    V_1
    IL_00df:  ldloc.s    V_0
    IL_00e1:  ldloc.s    V_6
    IL_00e3:  dup
    IL_00e4:  ldc.i4.s   1
    IL_00e6:  sub
    IL_00e7:  stloc.s    V_6
    IL_00e9:  sub
    IL_00ea:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_00ef:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_00f4:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_00f9:  brfalse    IL_0203

    IL_00fe:  ldloc.s    V_6
    IL_0100:  ldc.i4.s   0
    IL_0102:  bgt        IL_00dd

    IL_0107:  ldloc.s    V_10
    IL_0109:  ldloc.s    V_0
    IL_010b:  sub
    IL_010c:  dup
    IL_010d:  stloc.s    V_7
    IL_010f:  ldc.i4.s   1
    IL_0111:  add
    IL_0112:  stloc.s    V_6
    IL_0114:  ldloc.s    V_6
    IL_0116:  ldc.i4.s   1
    IL_0118:  sub
    IL_0119:  dup
    IL_011a:  stloc.s    V_6
    IL_011c:  ldc.i4.s   0
    IL_011e:  ble        IL_0148

    IL_0123:  ldloc.s    V_1
    IL_0125:  ldloc.s    V_0
    IL_0127:  dup
    IL_0128:  ldc.i4.s   1
    IL_012a:  add
    IL_012b:  stloc.s    V_0
    IL_012d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0132:  ldstr      bytearray (00 00 02 00 09 00 2D 00 2F 00 00 00 02 00 04 00   // ......-./.......
                                    05 00 03 00 01 00 09 00 13 00 00 00 ) 
    IL_0137:  call       bool [System]System.Text.RegularExpressions.RegexRunner::CharInClass(char,
                                                                                              string)
    IL_013c:  brtrue     IL_0114

    IL_0141:  ldloc.s    V_0
    IL_0143:  ldc.i4.s   1
    IL_0145:  sub
    IL_0146:  stloc.s    V_0
    IL_0148:  ldloc.s    V_7
    IL_014a:  ldloc.s    V_6
    IL_014c:  ble.s      IL_017b

    IL_014e:  ldloc.s    V_3
    IL_0150:  ldloc.s    V_2
    IL_0152:  ldc.i4.1
    IL_0153:  sub
    IL_0154:  dup
    IL_0155:  stloc.s    V_2
    IL_0157:  ldloc.s    V_7
    IL_0159:  ldloc.s    V_6
    IL_015b:  sub
    IL_015c:  ldc.i4.s   1
    IL_015e:  sub
    IL_015f:  stelem.i4
    IL_0160:  ldloc.s    V_3
    IL_0162:  ldloc.s    V_2
    IL_0164:  ldc.i4.1
    IL_0165:  sub
    IL_0166:  dup
    IL_0167:  stloc.s    V_2
    IL_0169:  ldloc.s    V_0
    IL_016b:  ldc.i4.s   1
    IL_016d:  sub
    IL_016e:  stelem.i4
    IL_016f:  ldloc.s    V_3
    IL_0171:  ldloc.s    V_2
    IL_0173:  ldc.i4.1
    IL_0174:  sub
    IL_0175:  dup
    IL_0176:  stloc.s    V_2
    IL_0178:  ldc.i4.s   2
    IL_017a:  stelem.i4
    IL_017b:  ldloc.s    V_5
    IL_017d:  ldloc.s    V_4
    IL_017f:  dup
    IL_0180:  ldc.i4.1
    IL_0181:  add
    IL_0182:  stloc.s    V_4
    IL_0184:  ldelem.i4
    IL_0185:  stloc.s    V_6
    IL_0187:  ldarg.0
    IL_0188:  ldc.i4.s   1
    IL_018a:  ldloc.s    V_6
    IL_018c:  ldloc.s    V_0
    IL_018e:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_0193:  ldloc.s    V_3
    IL_0195:  ldloc.s    V_2
    IL_0197:  ldc.i4.1
    IL_0198:  sub
    IL_0199:  dup
    IL_019a:  stloc.s    V_2
    IL_019c:  ldloc.s    V_6
    IL_019e:  stelem.i4
    IL_019f:  ldloc.s    V_3
    IL_01a1:  ldloc.s    V_2
    IL_01a3:  ldc.i4.1
    IL_01a4:  sub
    IL_01a5:  dup
    IL_01a6:  stloc.s    V_2
    IL_01a8:  ldc.i4.s   3
    IL_01aa:  stelem.i4
    IL_01ab:  ldloc.s    V_0
    IL_01ad:  ldloc.s    V_10
    IL_01af:  bge        IL_0203

    IL_01b4:  ldloc.s    V_1
    IL_01b6:  ldloc.s    V_0
    IL_01b8:  dup
    IL_01b9:  ldc.i4.s   1
    IL_01bb:  add
    IL_01bc:  stloc.s    V_0
    IL_01be:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_01c3:  ldc.i4.s   36
    IL_01c5:  bne.un     IL_0203

    IL_01ca:  ldloc.s    V_5
    IL_01cc:  ldloc.s    V_4
    IL_01ce:  dup
    IL_01cf:  ldc.i4.1
    IL_01d0:  add
    IL_01d1:  stloc.s    V_4
    IL_01d3:  ldelem.i4
    IL_01d4:  stloc.s    V_6
    IL_01d6:  ldarg.0
    IL_01d7:  ldc.i4.s   0
    IL_01d9:  ldloc.s    V_6
    IL_01db:  ldloc.s    V_0
    IL_01dd:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Capture(int32,
                                                                                                   int32,
                                                                                                   int32)
    IL_01e2:  ldloc.s    V_3
    IL_01e4:  ldloc.s    V_2
    IL_01e6:  ldc.i4.1
    IL_01e7:  sub
    IL_01e8:  dup
    IL_01e9:  stloc.s    V_2
    IL_01eb:  ldloc.s    V_6
    IL_01ed:  stelem.i4
    IL_01ee:  ldloc.s    V_3
    IL_01f0:  ldloc.s    V_2
    IL_01f2:  ldc.i4.1
    IL_01f3:  sub
    IL_01f4:  dup
    IL_01f5:  stloc.s    V_2
    IL_01f7:  ldc.i4.s   3
    IL_01f9:  stelem.i4
    IL_01fa:  ldarg.0
    IL_01fb:  ldloc.s    V_0
    IL_01fd:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0202:  ret

    IL_0203:  ldarg.0
    IL_0204:  ldloc.s    V_2
    IL_0206:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_020b:  ldarg.0
    IL_020c:  ldloc.s    V_4
    IL_020e:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0213:  ldarg.0
    IL_0214:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::EnsureStorage()
    IL_0219:  ldarg.0
    IL_021a:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackpos
    IL_021f:  stloc.s    V_2
    IL_0221:  ldarg.0
    IL_0222:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runstackpos
    IL_0227:  stloc.s    V_4
    IL_0229:  ldarg.0
    IL_022a:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runtrack
    IL_022f:  stloc.s    V_3
    IL_0231:  ldarg.0
    IL_0232:  ldfld      int32[] [System]System.Text.RegularExpressions.RegexRunner::runstack
    IL_0237:  stloc.s    V_5
    IL_0239:  ldloc.s    V_3
    IL_023b:  ldloc.s    V_2
    IL_023d:  dup
    IL_023e:  ldc.i4.1
    IL_023f:  add
    IL_0240:  stloc.s    V_2
    IL_0242:  ldelem.i4
    IL_0243:  switch     ( 
                          IL_0258,
                          IL_0269,
                          IL_0275,
                          IL_02c5)
    IL_0258:  ldloc.s    V_3
    IL_025a:  ldloc.s    V_2
    IL_025c:  dup
    IL_025d:  ldc.i4.1
    IL_025e:  add
    IL_025f:  stloc.s    V_2
    IL_0261:  ldelem.i4
    IL_0262:  stloc.s    V_0
    IL_0264:  br         IL_01fa

    IL_0269:  ldloc.s    V_4
    IL_026b:  ldc.i4.s   1
    IL_026d:  add
    IL_026e:  stloc.s    V_4
    IL_0270:  br         IL_0203

    IL_0275:  ldloc.s    V_3
    IL_0277:  ldloc.s    V_2
    IL_0279:  dup
    IL_027a:  ldc.i4.1
    IL_027b:  add
    IL_027c:  stloc.s    V_2
    IL_027e:  ldelem.i4
    IL_027f:  stloc.s    V_0
    IL_0281:  ldloc.s    V_3
    IL_0283:  ldloc.s    V_2
    IL_0285:  dup
    IL_0286:  ldc.i4.1
    IL_0287:  add
    IL_0288:  stloc.s    V_2
    IL_028a:  ldelem.i4
    IL_028b:  stloc.s    V_6
    IL_028d:  ldloc.s    V_6
    IL_028f:  ldc.i4.s   0
    IL_0291:  ble        IL_017b

    IL_0296:  ldloc.s    V_3
    IL_0298:  ldloc.s    V_2
    IL_029a:  ldc.i4.1
    IL_029b:  sub
    IL_029c:  dup
    IL_029d:  stloc.s    V_2
    IL_029f:  ldloc.s    V_6
    IL_02a1:  ldc.i4.s   1
    IL_02a3:  sub
    IL_02a4:  stelem.i4
    IL_02a5:  ldloc.s    V_3
    IL_02a7:  ldloc.s    V_2
    IL_02a9:  ldc.i4.1
    IL_02aa:  sub
    IL_02ab:  dup
    IL_02ac:  stloc.s    V_2
    IL_02ae:  ldloc.s    V_0
    IL_02b0:  ldc.i4.s   1
    IL_02b2:  sub
    IL_02b3:  stelem.i4
    IL_02b4:  ldloc.s    V_3
    IL_02b6:  ldloc.s    V_2
    IL_02b8:  ldc.i4.1
    IL_02b9:  sub
    IL_02ba:  dup
    IL_02bb:  stloc.s    V_2
    IL_02bd:  ldc.i4.s   2
    IL_02bf:  stelem.i4
    IL_02c0:  br         IL_017b

    IL_02c5:  ldloc.s    V_5
    IL_02c7:  ldloc.s    V_4
    IL_02c9:  ldc.i4.1
    IL_02ca:  sub
    IL_02cb:  dup
    IL_02cc:  stloc.s    V_4
    IL_02ce:  ldloc.s    V_3
    IL_02d0:  ldloc.s    V_2
    IL_02d2:  dup
    IL_02d3:  ldc.i4.1
    IL_02d4:  add
    IL_02d5:  stloc.s    V_2
    IL_02d7:  ldelem.i4
    IL_02d8:  stelem.i4
    IL_02d9:  ldarg.0
    IL_02da:  callvirt   instance void [System]System.Text.RegularExpressions.RegexRunner::Uncapture()
    IL_02df:  br         IL_0203
  } // end of method BracketedControllerClosePatternRunner4::Go

  .method public virtual instance bool  FindFirstChar() cil managed
  {
    // Code size       887 (0x377)
    .maxstack  78
    .locals init (int32 V_0,
             string V_1,
             int32 V_2,
             int32 V_3)
    IL_0000:  ldarg.0
    IL_0001:  ldfld      string [System]System.Text.RegularExpressions.RegexRunner::runtext
    IL_0006:  stloc.s    V_1
    IL_0008:  ldarg.0
    IL_0009:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_000e:  stloc.s    V_3
    IL_0010:  ldarg.0
    IL_0011:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0016:  ldc.i4.s   1
    IL_0018:  add
    IL_0019:  stloc.s    V_0
    IL_001b:  br.s       IL_0024

    IL_001d:  ldc.i4.s   2
    IL_001f:  ldloc.s    V_0
    IL_0021:  add
    IL_0022:  stloc.s    V_0
    IL_0024:  ldloc.s    V_0
    IL_0026:  ldloc.s    V_3
    IL_0028:  bge        IL_0368

    IL_002d:  ldloc.s    V_1
    IL_002f:  ldloc.s    V_0
    IL_0031:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0036:  dup
    IL_0037:  stloc.s    V_2
    IL_0039:  ldc.i4.s   58
    IL_003b:  beq        IL_033f

    IL_0040:  ldloc.s    V_2
    IL_0042:  ldc.i4.s   58
    IL_0044:  sub
    IL_0045:  dup
    IL_0046:  stloc.s    V_2
    IL_0048:  ldc.i4.s   67
    IL_004a:  bgt.un.s   IL_001d

    IL_004c:  ldloc.s    V_2
    IL_004e:  switch     ( 
                          IL_0163,
                          IL_016a,
                          IL_0171,
                          IL_0178,
                          IL_017f,
                          IL_0186,
                          IL_018d,
                          IL_0194,
                          IL_019b,
                          IL_01a2,
                          IL_01a9,
                          IL_01b0,
                          IL_01b7,
                          IL_01be,
                          IL_01c5,
                          IL_01cc,
                          IL_01d3,
                          IL_01da,
                          IL_01e1,
                          IL_01e8,
                          IL_01ef,
                          IL_01f6,
                          IL_01fd,
                          IL_0204,
                          IL_020b,
                          IL_0212,
                          IL_0219,
                          IL_0220,
                          IL_0227,
                          IL_022e,
                          IL_0235,
                          IL_023c,
                          IL_0243,
                          IL_024a,
                          IL_0251,
                          IL_0258,
                          IL_025f,
                          IL_0266,
                          IL_026d,
                          IL_0274,
                          IL_027b,
                          IL_0282,
                          IL_0289,
                          IL_0290,
                          IL_0297,
                          IL_029e,
                          IL_02a5,
                          IL_02ac,
                          IL_02b3,
                          IL_02ba,
                          IL_02c1,
                          IL_02c8,
                          IL_02cf,
                          IL_02d6,
                          IL_02dd,
                          IL_02e4,
                          IL_02eb,
                          IL_02f2,
                          IL_02f9,
                          IL_0300,
                          IL_0307,
                          IL_030e,
                          IL_0315,
                          IL_031c,
                          IL_0323,
                          IL_032a,
                          IL_0331,
                          IL_0338)
    IL_0163:  ldc.i4.s   0
    IL_0165:  br         IL_001f

    IL_016a:  ldc.i4.s   2
    IL_016c:  br         IL_001f

    IL_0171:  ldc.i4.s   2
    IL_0173:  br         IL_001f

    IL_0178:  ldc.i4.s   2
    IL_017a:  br         IL_001f

    IL_017f:  ldc.i4.s   2
    IL_0181:  br         IL_001f

    IL_0186:  ldc.i4.s   2
    IL_0188:  br         IL_001f

    IL_018d:  ldc.i4.s   2
    IL_018f:  br         IL_001f

    IL_0194:  ldc.i4.s   2
    IL_0196:  br         IL_001f

    IL_019b:  ldc.i4.s   2
    IL_019d:  br         IL_001f

    IL_01a2:  ldc.i4.s   2
    IL_01a4:  br         IL_001f

    IL_01a9:  ldc.i4.s   2
    IL_01ab:  br         IL_001f

    IL_01b0:  ldc.i4.s   2
    IL_01b2:  br         IL_001f

    IL_01b7:  ldc.i4.s   2
    IL_01b9:  br         IL_001f

    IL_01be:  ldc.i4.s   2
    IL_01c0:  br         IL_001f

    IL_01c5:  ldc.i4.s   2
    IL_01c7:  br         IL_001f

    IL_01cc:  ldc.i4.s   2
    IL_01ce:  br         IL_001f

    IL_01d3:  ldc.i4.s   2
    IL_01d5:  br         IL_001f

    IL_01da:  ldc.i4.s   2
    IL_01dc:  br         IL_001f

    IL_01e1:  ldc.i4.s   2
    IL_01e3:  br         IL_001f

    IL_01e8:  ldc.i4.s   2
    IL_01ea:  br         IL_001f

    IL_01ef:  ldc.i4.s   2
    IL_01f1:  br         IL_001f

    IL_01f6:  ldc.i4.s   2
    IL_01f8:  br         IL_001f

    IL_01fd:  ldc.i4.s   2
    IL_01ff:  br         IL_001f

    IL_0204:  ldc.i4.s   2
    IL_0206:  br         IL_001f

    IL_020b:  ldc.i4.s   2
    IL_020d:  br         IL_001f

    IL_0212:  ldc.i4.s   2
    IL_0214:  br         IL_001f

    IL_0219:  ldc.i4.s   2
    IL_021b:  br         IL_001f

    IL_0220:  ldc.i4.s   2
    IL_0222:  br         IL_001f

    IL_0227:  ldc.i4.s   2
    IL_0229:  br         IL_001f

    IL_022e:  ldc.i4.s   2
    IL_0230:  br         IL_001f

    IL_0235:  ldc.i4.s   2
    IL_0237:  br         IL_001f

    IL_023c:  ldc.i4.s   2
    IL_023e:  br         IL_001f

    IL_0243:  ldc.i4.s   2
    IL_0245:  br         IL_001f

    IL_024a:  ldc.i4.s   2
    IL_024c:  br         IL_001f

    IL_0251:  ldc.i4.s   2
    IL_0253:  br         IL_001f

    IL_0258:  ldc.i4.s   2
    IL_025a:  br         IL_001f

    IL_025f:  ldc.i4.s   2
    IL_0261:  br         IL_001f

    IL_0266:  ldc.i4.s   2
    IL_0268:  br         IL_001f

    IL_026d:  ldc.i4.s   2
    IL_026f:  br         IL_001f

    IL_0274:  ldc.i4.s   2
    IL_0276:  br         IL_001f

    IL_027b:  ldc.i4.s   2
    IL_027d:  br         IL_001f

    IL_0282:  ldc.i4.s   2
    IL_0284:  br         IL_001f

    IL_0289:  ldc.i4.s   2
    IL_028b:  br         IL_001f

    IL_0290:  ldc.i4.s   2
    IL_0292:  br         IL_001f

    IL_0297:  ldc.i4.s   2
    IL_0299:  br         IL_001f

    IL_029e:  ldc.i4.s   2
    IL_02a0:  br         IL_001f

    IL_02a5:  ldc.i4.s   2
    IL_02a7:  br         IL_001f

    IL_02ac:  ldc.i4.s   2
    IL_02ae:  br         IL_001f

    IL_02b3:  ldc.i4.s   2
    IL_02b5:  br         IL_001f

    IL_02ba:  ldc.i4.s   2
    IL_02bc:  br         IL_001f

    IL_02c1:  ldc.i4.s   2
    IL_02c3:  br         IL_001f

    IL_02c8:  ldc.i4.s   2
    IL_02ca:  br         IL_001f

    IL_02cf:  ldc.i4.s   2
    IL_02d1:  br         IL_001f

    IL_02d6:  ldc.i4.s   2
    IL_02d8:  br         IL_001f

    IL_02dd:  ldc.i4.s   2
    IL_02df:  br         IL_001f

    IL_02e4:  ldc.i4.s   2
    IL_02e6:  br         IL_001f

    IL_02eb:  ldc.i4.s   2
    IL_02ed:  br         IL_001f

    IL_02f2:  ldc.i4.s   2
    IL_02f4:  br         IL_001f

    IL_02f9:  ldc.i4.s   2
    IL_02fb:  br         IL_001f

    IL_0300:  ldc.i4.s   2
    IL_0302:  br         IL_001f

    IL_0307:  ldc.i4.s   2
    IL_0309:  br         IL_001f

    IL_030e:  ldc.i4.s   2
    IL_0310:  br         IL_001f

    IL_0315:  ldc.i4.s   2
    IL_0317:  br         IL_001f

    IL_031c:  ldc.i4.s   2
    IL_031e:  br         IL_001f

    IL_0323:  ldc.i4.s   2
    IL_0325:  br         IL_001f

    IL_032a:  ldc.i4.s   2
    IL_032c:  br         IL_001f

    IL_0331:  ldc.i4.s   2
    IL_0333:  br         IL_001f

    IL_0338:  ldc.i4.s   1
    IL_033a:  br         IL_001f

    IL_033f:  ldloc.s    V_0
    IL_0341:  stloc.s    V_2
    IL_0343:  ldloc.s    V_1
    IL_0345:  ldloc.s    V_2
    IL_0347:  ldc.i4.s   1
    IL_0349:  sub
    IL_034a:  dup
    IL_034b:  stloc.s    V_2
    IL_034d:  callvirt   instance char [mscorlib]System.String::get_Chars(int32)
    IL_0352:  ldc.i4.s   125
    IL_0354:  beq.s      IL_035d

    IL_0356:  ldc.i4.s   1
    IL_0358:  br         IL_001f

    IL_035d:  ldarg.0
    IL_035e:  ldloc.s    V_2
    IL_0360:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0365:  ldc.i4.s   1
    IL_0367:  ret

    IL_0368:  ldarg.0
    IL_0369:  ldarg.0
    IL_036a:  ldfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextend
    IL_036f:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtextpos
    IL_0374:  ldc.i4.s   0
    IL_0376:  ret
  } // end of method BracketedControllerClosePatternRunner4::FindFirstChar

  .method public virtual instance void  InitTrackCount() cil managed
  {
    // Code size       9 (0x9)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.s   6
    IL_0003:  stfld      int32 [System]System.Text.RegularExpressions.RegexRunner::runtrackcount
    IL_0008:  ret
  } // end of method BracketedControllerClosePatternRunner4::InitTrackCount

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunner::.ctor()
    IL_0006:  ret
  } // end of method BracketedControllerClosePatternRunner4::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerClosePatternRunner4

.class private auto ansi Xeora.Web.RegularExpression.BracketedControllerClosePatternFactory4
       extends [System]System.Text.RegularExpressions.RegexRunnerFactory
{
  .method public virtual instance class [System]System.Text.RegularExpressions.RegexRunner 
          CreateInstance() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  1
    IL_0000:  newobj     instance void Xeora.Web.RegularExpression.BracketedControllerClosePatternRunner4::.ctor()
    IL_0005:  ret
  } // end of method BracketedControllerClosePatternFactory4::CreateInstance

  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       7 (0x7)
    .maxstack  2
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.RegexRunnerFactory::.ctor()
    IL_0006:  ret
  } // end of method BracketedControllerClosePatternFactory4::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerClosePatternFactory4

.class public auto ansi Xeora.Web.RegularExpression.BracketedControllerClosePattern
       extends [System]System.Text.RegularExpressions.Regex
{
  .method public specialname rtspecialname 
          instance void  .ctor() cil managed
  {
    // Code size       149 (0x95)
    .maxstack  5
    IL_0000:  ldarg.0
    IL_0001:  call       instance void [System]System.Text.RegularExpressions.Regex::.ctor()
    IL_0006:  ldarg.0
    IL_0007:  ldstr      "\\}:(\?<ItemID>[\\.\\w\\-]+)\\$"
    IL_000c:  stfld      string [System]System.Text.RegularExpressions.Regex::pattern
    IL_0011:  ldarg.0
    IL_0012:  ldc.i4.s   2
    IL_0014:  stfld      valuetype [System]System.Text.RegularExpressions.RegexOptions [System]System.Text.RegularExpressions.Regex::roptions
    IL_0019:  ldarg.0
    IL_001a:  newobj     instance void Xeora.Web.RegularExpression.BracketedControllerClosePatternFactory4::.ctor()
    IL_001f:  stfld      class [System]System.Text.RegularExpressions.RegexRunnerFactory [System]System.Text.RegularExpressions.Regex::factory
    IL_0024:  ldarg.0
    IL_0025:  newobj     instance void [mscorlib]System.Collections.Hashtable::.ctor()
    IL_002a:  stfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_002f:  ldarg.0
    IL_0030:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_0035:  ldstr      "0"
    IL_003a:  ldc.i4.s   0
    IL_003c:  box        [mscorlib]System.Int32
    IL_0041:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_0046:  ldarg.0
    IL_0047:  ldfld      class [mscorlib]System.Collections.Hashtable [System]System.Text.RegularExpressions.Regex::capnames
    IL_004c:  ldstr      "ItemID"
    IL_0051:  ldc.i4.s   1
    IL_0053:  box        [mscorlib]System.Int32
    IL_0058:  callvirt   instance void [mscorlib]System.Collections.Hashtable::Add(object,
                                                                                   object)
    IL_005d:  ldarg.0
    IL_005e:  ldc.i4.s   2
    IL_0060:  newarr     [mscorlib]System.String
    IL_0065:  stfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_006a:  ldarg.0
    IL_006b:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_0070:  ldc.i4.s   0
    IL_0072:  ldstr      "0"
    IL_0077:  stelem.ref
    IL_0078:  ldarg.0
    IL_0079:  ldfld      string[] [System]System.Text.RegularExpressions.Regex::capslist
    IL_007e:  ldc.i4.s   1
    IL_0080:  ldstr      "ItemID"
    IL_0085:  stelem.ref
    IL_0086:  ldarg.0
    IL_0087:  ldc.i4.s   2
    IL_0089:  stfld      int32 [System]System.Text.RegularExpressions.Regex::capsize
    IL_008e:  ldarg.0
    IL_008f:  call       instance void [System]System.Text.RegularExpressions.Regex::InitializeReferences()
    IL_0094:  ret
  } // end of method BracketedControllerClosePattern::.ctor

} // end of class Xeora.Web.RegularExpression.BracketedControllerClosePattern


// =============================================================

// *********** DISASSEMBLY COMPLETE ***********************
