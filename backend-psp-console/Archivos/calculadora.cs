using System;

namespace PSP.Archivos
{
    public class Calculadora
    {
        public int NumeroUno { get; set; }
        public int NumeroDos { get; set; }
        public double Resultado { get; private set; }
        public string Operacion { get; set; }

        public Calculadora(int numeroUno, int numeroDos, string operacion)
        {
            NumeroUno = numeroUno;
            NumeroDos = numeroDos;
            Operacion = operacion;
        }

        public double Suma(int a, int b)
        {
            Resultado = a + b;
            return Resultado;
        }

        public double Resta(int a, int b)
        {
            Resultado = a - b;
            return Resultado;
        }

        public double Multiplicacion(int a, int b)
        {
            Resultado = a * b;
            return Resultado;
        }

        public double Division(int a, int b)
        {
            if (b == 0)
            {
                Console.WriteLine("No se puede dividir por cero");
                Resultado = double.NaN; // o podrías lanzar una excepción
            }
            else
            {
                Resultado = (double)a / b; // asegurar división de punto flotante
            }

            return Resultado;
        }

        public double Calculo()
        {
            return Operacion switch
            {
                "+" => Suma(NumeroUno, NumeroDos),
                "-" => Resta(NumeroUno, NumeroDos),
                "*" => Multiplicacion(NumeroUno, NumeroDos),
                "/" => Division(NumeroUno, NumeroDos),
                _ => throw new InvalidOperationException("Operación no válida")
            };
        }
    }
}



