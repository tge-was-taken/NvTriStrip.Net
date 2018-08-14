using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NvTriStripDotNet.Benchmark
{
    internal static class Program
    {
        public const int TEST_COUNT = 100;
        private static readonly NvStripifier sStripifier = new NvStripifier();

        private static void Main( string[] args )
        {
            var indices = ParseObjFaces( "teapot.obj" );
            VerifyCorrectness( indices );
            RunBench( indices );

            Console.WriteLine( "Press any key to exit." );
            Console.ReadKey();
        }

        private static ushort[] ParseObjFaces( string filepath )
        {
            var indices = new List<ushort>();

            using ( var reader = File.OpenText( filepath ) )
            {
                while ( !reader.EndOfStream )
                {
                    var line = reader.ReadLine();
                    if ( line == null )
                        continue;

                    var tokens = line.Split( new[] { " " }, StringSplitOptions.RemoveEmptyEntries );
                    if ( tokens.Length == 0 || tokens[0] != "f" )
                        continue;

                    for ( int i = 0; i < 3; i++ )
                        indices.Add( ushort.Parse( tokens[1 + i] ) );
                }
            }

            return indices.ToArray();
        }

        private static void VerifyCorrectness(ushort[] indices)
        {
            sStripifier.GenerateStrips( indices, out var a );
            ManagedNvTriStrip.NvTriStripUtility.GenerateStrips( indices, out var b );

            Trace.Assert( a.Length == b.Length );

            for ( int i = 0; i < a.Length; i++ )
                Trace.Assert( a[ i ].Indices.SequenceEqual( b[ i ].Indices ) );

            Console.WriteLine( "Correctness test passed." );
        }

        private static void RunBench(ushort[] indices)
        {
            GC.Collect();
            GC.Collect();

            {
                var timer = Stopwatch.StartNew();
                for ( int i = 0; i < TEST_COUNT; i++ )
                    RunDotNetTest( indices );

                timer.Stop();
                Console.WriteLine( $"NvTriStripDotNet: {TEST_COUNT} iterations completed in {timer.ElapsedMilliseconds}ms (avg. {timer.ElapsedMilliseconds / TEST_COUNT}ms)" );
            }

            {
                var timer = Stopwatch.StartNew();
                for ( int i = 0; i < TEST_COUNT; i++ )
                    RunManagedTest( indices );

                timer.Stop();
                Console.WriteLine( $"ManagedNvTriStrip: {TEST_COUNT} iterations completed in {timer.ElapsedMilliseconds}ms (avg. {timer.ElapsedMilliseconds / TEST_COUNT}ms)" );
            }
        }

        private static void RunDotNetTest( ushort[] indices )
        {
            sStripifier.GenerateStrips( indices, out _ );
        }

        private static void RunManagedTest( ushort[] indices )
        {
            ManagedNvTriStrip.NvTriStripUtility.GenerateStrips( indices, out _ );
        }
    }
}
