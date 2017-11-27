@Echo Off

Del Xeora.Web.RegularExpression.il
Del Xeora.Web.RegularExpression.dll.unsign

ildasm Xeora.Web.RegularExpression.dll /out:Xeora.Web.RegularExpression.il
ren Xeora.Web.RegularExpression.dll Xeora.Web.RegularExpression.dll.unsign
ilasm Xeora.Web.RegularExpression.il /dll /key=Key.snk