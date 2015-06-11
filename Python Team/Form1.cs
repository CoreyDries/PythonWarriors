using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Windows.Forms;

namespace Python_Team
{

    public partial class Form1 : Form
    {

        private ScriptEngine pyEngine;
        private ScriptScope pyScope;

        public class Obj
        {
            private int num;

            // Objects passed to python need to be defualt constructable
            public Obj( int num = 1 )
            {
                this.num = num;
            }

            public int getNum()
            {
                return num;
            }

            public void inc()
            {
                ++num;
                Console.Out.WriteLine( "Obj::inc() --- " + num );
            }
        }

        private dynamic obj = new Obj( 3 );

        public Form1()
        {
            InitializeComponent();

            pyEngine = Python.CreateEngine();
            pyScope = pyEngine.CreateScope();

            pyScope.SetVariable( "obj", obj );

            Console.Out.WriteLine( obj.getNum() );

            obj.inc();

            CompilePython( "print obj.getNum()" ).Execute( pyScope );
            CompilePython( "obj.inc()" ).Execute( pyScope );

            Console.Out.WriteLine(
                CompilePython( "obj.getNum()" ).Execute( pyScope )
                );

            obj.inc();

            Console.Out.WriteLine(
                CompilePython( "obj" ).Execute( pyScope ).getNum()
                );

        }

        private CompiledCode CompilePython( string pythonCode )
        {
            return pyEngine.CreateScriptSourceFromString( pythonCode ).Compile();
        }

    }

}
