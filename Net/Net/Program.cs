using System;

namespace Net
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            try
            {
                TCPTest.Start();
            }
            catch (Exception ex)
            {
                Util.Log("Error:{0}.", ex);
            }
            finally
            {
                Console.ReadKey();
            }
        }
    }
}