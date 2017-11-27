using System.Collections.Generic;

namespace Xeora.Extension.Executable
{
    public class ClassInfo
    {
        private List<ClassInfo> _Classes;
        private List<MethodInfo> _Methods;

        public ClassInfo(string ID)
        {
            this.ID = ID;
            this._Classes = new List<ClassInfo>();
            this._Methods = new List<MethodInfo>();

            this.ClassesTouched = false;
            this.MethodsTouched = false;
        }

        public string ID { get; private set; }
        public ClassInfo[] Classes => this._Classes.ToArray();

        public ClassInfo AddClassInfo(string ID)
        {
            for (int cIC = this._Classes.Count - 1; cIC >= 0; cIC--)
            {
                ClassInfo cI = this._Classes[cIC];

                if (string.Compare(cI.ID, ID) == 0)
                    this._Classes.RemoveAt(cIC);
            }

            ClassInfo rClassInfo = new ClassInfo(ID);

            this._Classes.Add(rClassInfo);
            this.ClassesTouched = true;

            return rClassInfo;
        }

        public bool ClassesTouched { get; private set; }
        public MethodInfo[] Methods => this._Methods.ToArray();

        public MethodInfo AddMethodInfo(string ID, string[] @params)
        {
            for (int mIC = this._Methods.Count - 1; mIC >= 0; mIC--)
            {
                MethodInfo mI = this._Methods[mIC];

                if (string.Compare(mI.ID, ID) == 0 && 
                    this.ListCompare(mI.Params, @params))
                    this._Methods.RemoveAt(mIC);
            }

            MethodInfo rMethodInfo = new MethodInfo(ID, @params);

            this._Methods.Add(rMethodInfo);
            this.MethodsTouched = true;

            return rMethodInfo;
        }

        public bool MethodsTouched { get; private set; }

        private bool ListCompare(string[] params1, string[] params2)
        {
            if (params1 == null && params2 == null)
                return true;
            else if (params1 == null || params2 == null)
                return false;
            else if (params1.Length != params2.Length)
                return false;
            else if (params1.Length == params2.Length)
            {
                for (int pC = 0; pC <= params1.Length - 1; pC++)
                {
                    if (string.Compare(params1[pC], params2[pC]) != 0)
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
