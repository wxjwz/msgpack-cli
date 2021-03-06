﻿

#region -- License Terms --
//
// MessagePack for CLI
//
// Copyright (C) 2016 FUJIWARA, Yusuke
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//
//        http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//
#endregion -- License Terms --

// This file is borrowed from UniRX (https://github.com/neuecc/UniRx/blob/master/Assets/UnitTests/UnitTests.tt)
#if !UNITY_METRO && !UNITY_4_5
using System;
using System.Collections.Generic;
using System.Linq;

using MsgPack;
using MsgPack.Serialization;

namespace MsgPack
{
	/// <summary>
	///		Represents a test class which groups related tests and holds their states.
	/// </summary>
	public partial class TestClass
	{
		/// <summary>
		///		The null object for <see cref="Action" /> typed property.
		/// </summary>
		private static readonly Action Nop = () => {};

		/// <summary>
		///		The null object for <see cref="Action{TestClassInstance, object}"/> typed property.
		/// </summary>
		private static readonly Action<TestClassInstance, object> NoInitialization = ( c, i ) => {};

		/// <summary>
		///		Gets the name of the test class.
		/// </summary>
		/// <value>
		///		The name of the test class. This value will not be <c>null</c>.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		///		Gets the delegate for fixture level setup routine.
		/// </summary>
		/// <value>
		///		The delegate for fixture level setup routine. This value will not be <c>null</c> even if the underlying test class does not have any fixture level setup routines.
		/// </value>
		public Action FixtureSetup { get; set; }

		/// <summary>
		///		Gets the delegate for fixture level cleanup routine.
		/// </summary>
		/// <value>
		///		The delegate for fixture level cleanup routine. This value will not be <c>null</c> even if the underlying test class does not have any fixture level cleanup routines.
		/// </value>
		public Action FixtureCleanup { get; set; }

		/// <summary>
		///		Gets the count of test methods in the test class.
		/// </summary>
		/// <value>
		///		The ount of test methods in the test class.
		/// </value>
		public int MethodCount { get; private set; }

		/// <summary>
		///		The delegate which instantiate "test class" instance.
		/// </summary>
		private readonly Func<object> _instanceFactory;

		/// <summary>
		///		The delegate which initializes <see cref="TestClassInstance" /> instance with "test class" instance.
		/// </summary>
		private readonly Action<TestClassInstance, object> _testClassInstanceInitializer;

		/// <summary>
		///		Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the test class.</param>
		/// <param name="instanceFactory">The delegate which instantiate "test class" instance.</param>
		/// <param name="methodCount">The ount of test methods in the test class.</param>
		/// <param name="testClassInstanceInitializer">The delegate which initializes <see cref="TestClassInstance" /> instance with "test class" instance.</param>
		/// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="instanceFactory"/> is <c>null</c>.</exception>
		public TestClass( string name, Func<object> instanceFactory, int methodCount, Action<TestClassInstance, object> testClassInstanceInitializer )
		{
			if ( String.IsNullOrEmpty( name ) )
			{
				throw new ArgumentException( "name cannot be null nor empty.", "name" );
			}

			if ( instanceFactory == null )
			{
				throw new ArgumentNullException( "instanceFactory" );
			}

			this.Name = name;
			this._instanceFactory = instanceFactory;
			this._testClassInstanceInitializer = testClassInstanceInitializer ?? NoInitialization;
			this.MethodCount = methodCount;
			this.FixtureSetup = Nop;
			this.FixtureCleanup = Nop;
		}

		/// <summary>
		///		Creates a new, initialized <see cref="TestClassInstance" /> which represents instantiated test class information.
		/// </summary>
		/// <returns>
		///		The new, initialized <see cref="TestClassInstance" /> which represents instantiated test class information.
		/// </returns>
		public TestClassInstance NewTest()
		{
			var instance = this._instanceFactory();
			var result = new TestClassInstance( this.MethodCount );
			this._testClassInstanceInitializer( result, instance );
			return result;
		}
	}

	/// <summary>
	///		Represents instantiated test class information.
	/// </summary>
	public partial class TestClassInstance
	{
		// A test class intance will be hold via delegate for its instance methods.

		/// <summary>
		///		The null object for <see cref="Action" /> typed property.
		/// </summary>
		private static readonly Action Nop = () => {};

		/// <summary>
		///		Gets the delegate for per test setup routine.
		/// </summary>
		/// <value>
		///		The delegate for per test setup routine. This value will not be <c>null</c> even if the underlying test class does not have any per test setup routines.
		/// </value>
		public Action TestSetup { get; set; }

		/// <summary>
		///		Gets the delegate for per test cleanup routine.
		/// </summary>
		/// <value>
		///		The delegate for per test cleanup routine. This value will not be <c>null</c> even if the underlying test class does not have any per test cleanup routines.
		/// </value>
		public Action TestCleanup { get; set; }

		/// <summary>
		///		Gets the list of the test methods.
		/// </summary>
		/// <value>
		///		The list of the test methods. This value will not be <c>null</c>.
		/// </value>
		public IList<TestMethod> TestMethods { get; private set; }

		/// <summary>
		///		Initializes a new instance.
		/// </summary>
		/// <param name="methodCount">The ount of test methods in the test class.</param>
		public TestClassInstance( int methodCount )
		{
			this.TestMethods = new List<TestMethod>( methodCount );
			this.TestSetup = Nop;
			this.TestCleanup = Nop;
		}
	}

	/// <summary>
	///		Represents a test method.
	/// </summary>
	public partial class TestMethod
	{
		// "test case" is not supported.

		/// <summary>
		///		Gets the name of the test method.
		/// </summary>
		/// <value>
		///		The name of the test method. This value will not be <c>null</c>.
		/// </value>
		public string Name { get; private set; }

		/// <summary>
		///		Gets the delegate for instance methnod which is test method itself
		/// </summary>
		/// <value>
		///		The delegate for instance methnod which is test method itself. This value will not be <c>null</c>.
		/// </value>
		public Action Method { get; private set; }

		/// <summary>
		///		Initializes a new instance.
		/// </summary>
		/// <param name="name">The name of the test method.</param>
		/// <param name="method">The delegate for instance methnod which is test method itself.</param>
		/// <exception cref="ArgumentException">The <paramref name="name"/> is <c>null</c> or empty.</exception>
		/// <exception cref="ArgumentNullException">The <paramref name="method"/> is <c>null</c>.</exception>
		public TestMethod( string name, Action method )
		{
			if ( String.IsNullOrEmpty( name ) )
			{
				throw new ArgumentException( "name cannot be null nor empty.", "name" );
			}

			if ( method == null )
			{
				throw new ArgumentNullException( "method" );
			}

			this.Name = name;
			this.Method = method;
		}
	}

	/// <summary>
	///		Implements running environment agnostics test driver features.
	/// </summary>
	public partial class TestDriver
	{
		/// <summary>
		///		Gets the list of the test classes.
		/// </summary>
		/// <value>
		///		The list of the test classes. This value will not be <c>null</c>.
		/// </value>
		protected IList<TestClass> TestClasses { get; private set; }

		/// <summary>
		///		Initializes a new instance.
		/// </summary>
		protected TestDriver()
		{
			this.TestClasses = new List<TestClass>( 44 );
			InitializeTestClasses( this.TestClasses );
		}

		/// <summary>
		///		Fills intialized <see cref="TestClass" /> to specified list.
		/// </summary>
		private static void InitializeTestClasses( IList<TestClass> testClasses )
		{
			{
				var testClass = 
					new TestClass( 
						"AotTest", 
						AotTestInitializer.CreateInstance, 
						10,
						AotTestInitializer.InitializeInstance
					 );
testClass.FixtureSetup = new Action( AotTest.SetupFixture );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"ArrayGenerationBasedEnumSerializerTest", 
						ArrayGenerationBasedEnumSerializerTestInitializer.CreateInstance, 
						66,
						ArrayGenerationBasedEnumSerializerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"ArrayGenerationBasedReflectionMessagePackSerializerTest", 
						ArrayGenerationBasedReflectionMessagePackSerializerTestInitializer.CreateInstance, 
						420,
						ArrayGenerationBasedReflectionMessagePackSerializerTestInitializer.InitializeInstance
					 );
testClass.FixtureSetup = new Action( ArrayGenerationBasedReflectionMessagePackSerializerTest.SetUpFixture );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"ArrayReflectionBasedEnumSerializerTest", 
						ArrayReflectionBasedEnumSerializerTestInitializer.CreateInstance, 
						72,
						ArrayReflectionBasedEnumSerializerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"ArrayReflectionBasedReflectionMessagePackSerializerTest", 
						ArrayReflectionBasedReflectionMessagePackSerializerTestInitializer.CreateInstance, 
						600,
						ArrayReflectionBasedReflectionMessagePackSerializerTestInitializer.InitializeInstance
					 );
testClass.FixtureSetup = new Action( ArrayReflectionBasedReflectionMessagePackSerializerTest.SetUpFixture );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"BigEndianBinaryTest", 
						BigEndianBinaryTestInitializer.CreateInstance, 
						10,
						BigEndianBinaryTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"ByteArrayPackerTest", 
						ByteArrayPackerTestInitializer.CreateInstance, 
						18,
						ByteArrayPackerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"DirectConversionTest", 
						DirectConversionTestInitializer.CreateInstance, 
						15,
						DirectConversionTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"EqualsTest", 
						EqualsTestInitializer.CreateInstance, 
						4,
						EqualsTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"GenerationBasedNilImplicationTest", 
						GenerationBasedNilImplicationTestInitializer.CreateInstance, 
						32,
						GenerationBasedNilImplicationTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"InheritanceTest", 
						InheritanceTestInitializer.CreateInstance, 
						1,
						InheritanceTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"KeyNameTransformersTest", 
						KeyNameTransformersTestInitializer.CreateInstance, 
						2,
						KeyNameTransformersTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MapGenerationBasedEnumSerializerTest", 
						MapGenerationBasedEnumSerializerTestInitializer.CreateInstance, 
						66,
						MapGenerationBasedEnumSerializerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MapGenerationBasedReflectionMessagePackSerializerTest", 
						MapGenerationBasedReflectionMessagePackSerializerTestInitializer.CreateInstance, 
						420,
						MapGenerationBasedReflectionMessagePackSerializerTestInitializer.InitializeInstance
					 );
testClass.FixtureSetup = new Action( MapGenerationBasedReflectionMessagePackSerializerTest.SetUpFixture );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MapReflectionBasedEnumSerializerTest", 
						MapReflectionBasedEnumSerializerTestInitializer.CreateInstance, 
						72,
						MapReflectionBasedEnumSerializerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MapReflectionBasedReflectionMessagePackSerializerTest", 
						MapReflectionBasedReflectionMessagePackSerializerTestInitializer.CreateInstance, 
						605,
						MapReflectionBasedReflectionMessagePackSerializerTestInitializer.InitializeInstance
					 );
testClass.FixtureSetup = new Action( MapReflectionBasedReflectionMessagePackSerializerTest.SetUpFixture );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackConvertTest", 
						MessagePackConvertTestInitializer.CreateInstance, 
						43,
						MessagePackConvertTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackExtendedTypeObjectTest", 
						MessagePackExtendedTypeObjectTestInitializer.CreateInstance, 
						12,
						MessagePackExtendedTypeObjectTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectDictionaryTest", 
						MessagePackObjectDictionaryTestInitializer.CreateInstance, 
						58,
						MessagePackObjectDictionaryTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest", 
						MessagePackObjectTestInitializer.CreateInstance, 
						24,
						MessagePackObjectTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_Equals", 
						MessagePackObjectTest_EqualsInitializer.CreateInstance, 
						631,
						MessagePackObjectTest_EqualsInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_Exceptionals", 
						MessagePackObjectTest_ExceptionalsInitializer.CreateInstance, 
						192,
						MessagePackObjectTest_ExceptionalsInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_IPackable", 
						MessagePackObjectTest_IPackableInitializer.CreateInstance, 
						55,
						MessagePackObjectTest_IPackableInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_IsTypeOf", 
						MessagePackObjectTest_IsTypeOfInitializer.CreateInstance, 
						544,
						MessagePackObjectTest_IsTypeOfInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_Miscs", 
						MessagePackObjectTest_MiscsInitializer.CreateInstance, 
						4,
						MessagePackObjectTest_MiscsInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_Object", 
						MessagePackObjectTest_ObjectInitializer.CreateInstance, 
						68,
						MessagePackObjectTest_ObjectInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackObjectTest_String", 
						MessagePackObjectTest_StringInitializer.CreateInstance, 
						26,
						MessagePackObjectTest_StringInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackSerializerTest", 
						MessagePackSerializerTestInitializer.CreateInstance, 
						17,
						MessagePackSerializerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackSerializerTTest", 
						MessagePackSerializerTTestInitializer.CreateInstance, 
						44,
						MessagePackSerializerTTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessagePackStringTest", 
						MessagePackStringTestInitializer.CreateInstance, 
						10,
						MessagePackStringTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"MessageUnpackableTest", 
						MessageUnpackableTestInitializer.CreateInstance, 
						1,
						MessageUnpackableTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"PackerFactoryTest", 
						PackerFactoryTestInitializer.CreateInstance, 
						3,
						PackerFactoryTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"PackUnpackTest", 
						PackUnpackTestInitializer.CreateInstance, 
						37,
						PackUnpackTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"ReflectionBasedNilImplicationTest", 
						ReflectionBasedNilImplicationTestInitializer.CreateInstance, 
						32,
						ReflectionBasedNilImplicationTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"RegressionTests", 
						RegressionTestsInitializer.CreateInstance, 
						13,
						RegressionTestsInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"SerializationContextTest", 
						SerializationContextTestInitializer.CreateInstance, 
						30,
						SerializationContextTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"StreamPackerTest", 
						StreamPackerTestInitializer.CreateInstance, 
						3,
						StreamPackerTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"StructWithDataContractTest", 
						StructWithDataContractTestInitializer.CreateInstance, 
						2,
						StructWithDataContractTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"UnpackerFactoryTest", 
						UnpackerFactoryTestInitializer.CreateInstance, 
						21,
						UnpackerFactoryTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"UnpackingTest_Ext", 
						UnpackingTest_ExtInitializer.CreateInstance, 
						126,
						UnpackingTest_ExtInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"UnpackingTest_Misc", 
						UnpackingTest_MiscInitializer.CreateInstance, 
						100,
						UnpackingTest_MiscInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"UnpackingTest_Raw", 
						UnpackingTest_RawInitializer.CreateInstance, 
						320,
						UnpackingTest_RawInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"UnpackingTest_Scalar", 
						UnpackingTest_ScalarInitializer.CreateInstance, 
						86,
						UnpackingTest_ScalarInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

			{
				var testClass = 
					new TestClass( 
						"VersioningTest", 
						VersioningTestInitializer.CreateInstance, 
						10,
						VersioningTestInitializer.InitializeInstance
					 );
				testClasses.Add( testClass );
			}

		} // void InitializeTestClasses
	} // partial class TestDriver
	internal static class AotTestInitializer
	{
		public static object CreateInstance()
		{
			return new AotTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( AotTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_ArraySegmentOfByte", new Action( instance.TestGenericDefaultSerializer_ArraySegmentOfByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_ArraySegmentOfChar", new Action( instance.TestGenericDefaultSerializer_ArraySegmentOfChar ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_ArraySegmentOfInt32", new Action( instance.TestGenericDefaultSerializer_ArraySegmentOfInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_Dictionary", new Action( instance.TestGenericDefaultSerializer_Dictionary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_KeyValuePair", new Action( instance.TestGenericDefaultSerializer_KeyValuePair ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_List", new Action( instance.TestGenericDefaultSerializer_List ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_ListOfMessagePackObject", new Action( instance.TestGenericDefaultSerializer_ListOfMessagePackObject ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_Queue", new Action( instance.TestGenericDefaultSerializer_Queue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGenericDefaultSerializer_Stack", new Action( instance.TestGenericDefaultSerializer_Stack ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeMetadataExtraction", new Action( instance.TestTypeMetadataExtraction ) ) );
		}
	} 

	internal static class ArrayGenerationBasedEnumSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new ArrayGenerationBasedEnumSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( ArrayGenerationBasedEnumSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByName", new Action( instance.TestEnumByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByName", new Action( instance.TestEnumByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByName", new Action( instance.TestEnumInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByName", new Action( instance.TestEnumInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByName", new Action( instance.TestEnumInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByName", new Action( instance.TestEnumInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByName", new Action( instance.TestEnumInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByName", new Action( instance.TestEnumInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_AllUpper", new Action( instance.TestEnumKeyTransformer_AllUpper ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Custom", new Action( instance.TestEnumKeyTransformer_Custom ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Default_AsIs", new Action( instance.TestEnumKeyTransformer_Default_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_LowerCamel", new Action( instance.TestEnumKeyTransformer_LowerCamel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByName", new Action( instance.TestEnumSByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByName", new Action( instance.TestEnumSByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByName", new Action( instance.TestEnumUInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByName", new Action( instance.TestEnumUInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByName", new Action( instance.TestEnumUInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByName", new Action( instance.TestEnumUInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByName", new Action( instance.TestEnumUInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByName", new Action( instance.TestEnumUInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue ) ) );
		}
	} 

	internal static class ArrayGenerationBasedReflectionMessagePackSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new ArrayGenerationBasedReflectionMessagePackSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( ArrayGenerationBasedReflectionMessagePackSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionKnownType_Success", new Action( instance.TestAbstractClassCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionNoAttribute_Success", new Action( instance.TestAbstractClassCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionRuntimeType_Success", new Action( instance.TestAbstractClassCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyKnownType_Success", new Action( instance.TestAbstractClassDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyNoAttribute_Fail", new Action( instance.TestAbstractClassDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyRuntimeType_Success", new Action( instance.TestAbstractClassDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemKnownType_Success", new Action( instance.TestAbstractClassListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemNoAttribute_Fail", new Action( instance.TestAbstractClassListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemRuntimeType_Success", new Action( instance.TestAbstractClassListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberKnownType_Success", new Action( instance.TestAbstractClassMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberNoAttribute_Fail", new Action( instance.TestAbstractClassMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberRuntimeType_Success", new Action( instance.TestAbstractClassMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_Default_Success", new Action( instance.TestAbstractTypes_KnownCollections_Default_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistration_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistration_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_WithoutRegistration_Fail", new Action( instance.TestAbstractTypes_KnownCollections_WithoutRegistration_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_NotACollection_Fail", new Action( instance.TestAbstractTypes_NotACollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeField", new Action( instance.TestAddOnlyCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArray", new Action( instance.TestAddOnlyCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArrayNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectField", new Action( instance.TestAddOnlyCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArray", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectField", new Action( instance.TestAddOnlyCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArray", new Action( instance.TestAddOnlyCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldNull", new Action( instance.TestAddOnlyCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteField", new Action( instance.TestArraySegmentByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteFieldArray", new Action( instance.TestArraySegmentByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalField", new Action( instance.TestArraySegmentDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalFieldArray", new Action( instance.TestArraySegmentDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32Field", new Action( instance.TestArraySegmentInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32FieldArray", new Action( instance.TestArraySegmentInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoDefaultConstructor_Packable", new Action( instance.TestAsymmetric_PackOnly_NoDefaultConstructor_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable", new Action( instance.TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownCollectionItem_Fail", new Action( instance.TestAttribute_DuplicatedKnownCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownDictionaryKey_Fail", new Action( instance.TestAttribute_DuplicatedKnownDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownMember_Fail", new Action( instance.TestAttribute_DuplicatedKnownMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeCollectionItem_Fail", new Action( instance.TestAttribute_KnownAndRuntimeCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeDictionaryKey_Fail", new Action( instance.TestAttribute_KnownAndRuntimeDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeMember_Fail", new Action( instance.TestAttribute_KnownAndRuntimeMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayContent", new Action( instance.TestByteArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayField", new Action( instance.TestByteArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArray", new Action( instance.TestByteArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArrayNull", new Action( instance.TestByteArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldNull", new Action( instance.TestByteArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteField", new Action( instance.TestByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteFieldArray", new Action( instance.TestByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayContent", new Action( instance.TestCharArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayField", new Action( instance.TestCharArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArray", new Action( instance.TestCharArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArrayNull", new Action( instance.TestCharArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldNull", new Action( instance.TestCharArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharField", new Action( instance.TestCharField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharFieldArray", new Action( instance.TestCharFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectField", new Action( instance.TestCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArray", new Action( instance.TestCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldNull", new Action( instance.TestCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_Aware", new Action( instance.TestCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_NotAware", new Action( instance.TestCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_Aware", new Action( instance.TestCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_NotAware", new Action( instance.TestCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Success", new Action( instance.TestCollection_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_Aware", new Action( instance.TestCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_NotAware", new Action( instance.TestCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeField", new Action( instance.TestCollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArray", new Action( instance.TestCollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArrayNull", new Action( instance.TestCollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldNull", new Action( instance.TestCollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectField", new Action( instance.TestCollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArray", new Action( instance.TestCollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArrayNull", new Action( instance.TestCollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldNull", new Action( instance.TestCollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstructorDeserializationWithParametersNotInLexicalOrder", new Action( instance.TestConstructorDeserializationWithParametersNotInLexicalOrder ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTime", new Action( instance.TestDateTime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeClassic", new Action( instance.TestDateTimeClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeField", new Action( instance.TestDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeFieldArray", new Action( instance.TestDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Local", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Utc", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Local", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Utc", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeNullableChangeOnDemand", new Action( instance.TestDateTimeNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffset", new Action( instance.TestDateTimeOffset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetClassic", new Action( instance.TestDateTimeOffsetClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetField", new Action( instance.TestDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetFieldArray", new Action( instance.TestDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetNullableChangeOnDemand", new Action( instance.TestDateTimeOffsetNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalField", new Action( instance.TestDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalFieldArray", new Action( instance.TestDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_Aware", new Action( instance.TestDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_NotAware", new Action( instance.TestDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_Aware", new Action( instance.TestDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_NotAware", new Action( instance.TestDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_Aware", new Action( instance.TestDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_NotAware", new Action( instance.TestDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryField", new Action( instance.TestDictionaryEntryField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryFieldArray", new Action( instance.TestDictionaryEntryFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectField", new Action( instance.TestDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArray", new Action( instance.TestDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArrayNull", new Action( instance.TestDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldNull", new Action( instance.TestDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeField", new Action( instance.TestDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArray", new Action( instance.TestDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldNull", new Action( instance.TestDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes", new Action( instance.TestEmptyBytes ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes_Classic", new Action( instance.TestEmptyBytes_Classic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyIntArray", new Action( instance.TestEmptyIntArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyKeyValuePairArray", new Action( instance.TestEmptyKeyValuePairArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyMap", new Action( instance.TestEmptyMap ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyString", new Action( instance.TestEmptyString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnum", new Action( instance.TestEnum ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_Aware", new Action( instance.TestEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_NotAware", new Action( instance.TestEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_Aware", new Action( instance.TestEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_NotAware", new Action( instance.TestEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_Aware", new Action( instance.TestEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_NotAware", new Action( instance.TestEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExplicitlyImplementedPackableUnpackable", new Action( instance.TestExplicitlyImplementedPackableUnpackable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ClassicContext", new Action( instance.TestExt_ClassicContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ContextWithPackerCompatilibyOptionsNone", new Action( instance.TestExt_ContextWithPackerCompatilibyOptionsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_DefaultContext", new Action( instance.TestExt_DefaultContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseField", new Action( instance.TestFalseField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseFieldArray", new Action( instance.TestFalseFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEField", new Action( instance.TestFILETIMEField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEFieldArray", new Action( instance.TestFILETIMEFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructor", new Action( instance.TestFullVersionConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArray", new Action( instance.TestFullVersionConstructorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArrayNull", new Action( instance.TestFullVersionConstructorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorNull", new Action( instance.TestFullVersionConstructorNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetOnlyAndConstructor", new Action( instance.TestGetOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGlobalNamespace", new Action( instance.TestGlobalNamespace ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidField", new Action( instance.TestGuidField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidFieldArray", new Action( instance.TestGuidFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectField", new Action( instance.TestHashSet_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArray", new Action( instance.TestHashSet_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArrayNull", new Action( instance.TestHashSet_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldNull", new Action( instance.TestHashSet_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeField", new Action( instance.TestHashSetDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArray", new Action( instance.TestHashSetDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArrayNull", new Action( instance.TestHashSetDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldNull", new Action( instance.TestHashSetDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectField", new Action( instance.TestHashSetObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArray", new Action( instance.TestHashSetObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArrayNull", new Action( instance.TestHashSetObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldNull", new Action( instance.TestHashSetObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectField", new Action( instance.TestICollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArray", new Action( instance.TestICollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArrayNull", new Action( instance.TestICollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldNull", new Action( instance.TestICollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeField", new Action( instance.TestICollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArray", new Action( instance.TestICollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArrayNull", new Action( instance.TestICollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldNull", new Action( instance.TestICollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectField", new Action( instance.TestICollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArray", new Action( instance.TestICollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArrayNull", new Action( instance.TestICollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldNull", new Action( instance.TestICollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectField", new Action( instance.TestIDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArray", new Action( instance.TestIDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArrayNull", new Action( instance.TestIDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldNull", new Action( instance.TestIDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeField", new Action( instance.TestIDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArray", new Action( instance.TestIDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestIDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldNull", new Action( instance.TestIDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryValueType_Success", new Action( instance.TestIDictionaryValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectField", new Action( instance.TestIList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArray", new Action( instance.TestIList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArrayNull", new Action( instance.TestIList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldNull", new Action( instance.TestIList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeField", new Action( instance.TestIListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArray", new Action( instance.TestIListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArrayNull", new Action( instance.TestIListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldNull", new Action( instance.TestIListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectField", new Action( instance.TestIListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArray", new Action( instance.TestIListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArrayNull", new Action( instance.TestIListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldNull", new Action( instance.TestIListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListValueType_Success", new Action( instance.TestIListValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_Field", new Action( instance.TestImage_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArray", new Action( instance.TestImage_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArrayNull", new Action( instance.TestImage_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldNull", new Action( instance.TestImage_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsNonGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsNonGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32", new Action( instance.TestInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64", new Action( instance.TestInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionKnownType_Success", new Action( instance.TestInterfaceCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionNoAttribute_Success", new Action( instance.TestInterfaceCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionRuntimeType_Success", new Action( instance.TestInterfaceCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyKnownType_Success", new Action( instance.TestInterfaceDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyNoAttribute_Fail", new Action( instance.TestInterfaceDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyRuntimeType_Success", new Action( instance.TestInterfaceDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemKnownType_Success", new Action( instance.TestInterfaceListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemNoAttribute_Fail", new Action( instance.TestInterfaceListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemRuntimeType_Success", new Action( instance.TestInterfaceListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberKnownType_Success", new Action( instance.TestInterfaceMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberNoAttribute_Fail", new Action( instance.TestInterfaceMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberRuntimeType_Success", new Action( instance.TestInterfaceMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_Plain", new Action( instance.TestIssue25_Plain ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_SelfComposite", new Action( instance.TestIssue25_SelfComposite ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetField", new Action( instance.TestKeyValuePairStringDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetFieldArray", new Action( instance.TestKeyValuePairStringDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Field_Known", new Action( instance.TestKnownType_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Property_Known", new Action( instance.TestKnownType_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Field_Known", new Action( instance.TestKnownType_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Property_Known", new Action( instance.TestKnownType_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectField", new Action( instance.TestList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArray", new Action( instance.TestList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArrayNull", new Action( instance.TestList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldNull", new Action( instance.TestList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_Aware", new Action( instance.TestList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_NotAware", new Action( instance.TestList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_Aware", new Action( instance.TestList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_NotAware", new Action( instance.TestList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_Aware", new Action( instance.TestList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_NotAware", new Action( instance.TestList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeField", new Action( instance.TestListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArray", new Action( instance.TestListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArrayNull", new Action( instance.TestListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldNull", new Action( instance.TestListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectField", new Action( instance.TestListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArray", new Action( instance.TestListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArrayNull", new Action( instance.TestListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldNull", new Action( instance.TestListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestManyMembers", new Action( instance.TestManyMembers ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteField", new Action( instance.TestMaxByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteFieldArray", new Action( instance.TestMaxByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32Field", new Action( instance.TestMaxInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32FieldArray", new Action( instance.TestMaxInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64Field", new Action( instance.TestMaxInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64FieldArray", new Action( instance.TestMaxInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16Field", new Action( instance.TestMaxUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16FieldArray", new Action( instance.TestMaxUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_Field", new Action( instance.TestMessagePackObject_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArray", new Action( instance.TestMessagePackObject_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArrayNull", new Action( instance.TestMessagePackObject_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldNull", new Action( instance.TestMessagePackObject_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_Field", new Action( instance.TestMessagePackObjectArray_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArray", new Action( instance.TestMessagePackObjectArray_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArrayNull", new Action( instance.TestMessagePackObjectArray_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldNull", new Action( instance.TestMessagePackObjectArray_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32Field", new Action( instance.TestMinInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32FieldArray", new Action( instance.TestMinInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64Field", new Action( instance.TestMinInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64FieldArray", new Action( instance.TestMinInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArray", new Action( instance.TestMultiDimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArrayComprex", new Action( instance.TestMultiDimensionalArrayComprex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection", new Action( instance.TestNameValueCollection ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection_NullKey", new Action( instance.TestNameValueCollection_NullKey ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_Fail", new Action( instance.TestNoMembers_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableFail", new Action( instance.TestNoMembers_PackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableUnpackableSuccess", new Action( instance.TestNoMembers_PackableUnpackableSuccess ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_UnpackableFail", new Action( instance.TestNoMembers_UnpackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_Aware", new Action( instance.TestNonGenericCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_NotAware", new Action( instance.TestNonGenericCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_Aware", new Action( instance.TestNonGenericCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_NotAware", new Action( instance.TestNonGenericCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_Aware", new Action( instance.TestNonGenericCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_NotAware", new Action( instance.TestNonGenericCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_Aware", new Action( instance.TestNonGenericDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_NotAware", new Action( instance.TestNonGenericDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_Aware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_NotAware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_Aware", new Action( instance.TestNonGenericDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_NotAware", new Action( instance.TestNonGenericDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_Aware", new Action( instance.TestNonGenericEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_NotAware", new Action( instance.TestNonGenericEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_Aware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_NotAware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_Aware", new Action( instance.TestNonGenericEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_NotAware", new Action( instance.TestNonGenericEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_Aware", new Action( instance.TestNonGenericList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_NotAware", new Action( instance.TestNonGenericList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_Aware", new Action( instance.TestNonGenericList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_NotAware", new Action( instance.TestNonGenericList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_Aware", new Action( instance.TestNonGenericList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_NotAware", new Action( instance.TestNonGenericList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_DataContract_Failed", new Action( instance.TestNonPublicType_DataContract_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_MessagePackMember_Failed", new Action( instance.TestNonPublicType_MessagePackMember_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_Plain_Failed", new Action( instance.TestNonPublicType_Plain_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_DataContract", new Action( instance.TestNonPublicWritableMember_DataContract ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_MessagePackMember", new Action( instance.TestNonPublicWritableMember_MessagePackMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_PlainOldCliClass", new Action( instance.TestNonPublicWritableMember_PlainOldCliClass ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonZeroBoundMultidimensionalArray", new Action( instance.TestNonZeroBoundMultidimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullable", new Action( instance.TestNullable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullField", new Action( instance.TestNullField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArray", new Action( instance.TestNullFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArrayNull", new Action( instance.TestNullFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldNull", new Action( instance.TestNullFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayField", new Action( instance.TestObjectArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArray", new Action( instance.TestObjectArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArrayNull", new Action( instance.TestObjectArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldNull", new Action( instance.TestObjectArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectField", new Action( instance.TestObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArray", new Action( instance.TestObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArrayNull", new Action( instance.TestObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldNull", new Action( instance.TestObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Null_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphismAttributesInType", new Action( instance.TestPolymorphismAttributesInType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestReadOnlyAndConstructor", new Action( instance.TestReadOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStaticMembersDoNotCausePrepareError", new Action( instance.TestStaticMembersDoNotCausePrepareError ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestString", new Action( instance.TestString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringField", new Action( instance.TestStringField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArray", new Action( instance.TestStringFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArrayNull", new Action( instance.TestStringFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldNull", new Action( instance.TestStringFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeField", new Action( instance.TestStringKeyedCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArray", new Action( instance.TestStringKeyedCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArrayNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectField", new Action( instance.TestStringKeyedCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArray", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectField", new Action( instance.TestStringKeyedCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArray", new Action( instance.TestStringKeyedCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldNull", new Action( instance.TestStringKeyedCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanField", new Action( instance.TestTimeSpanField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanFieldArray", new Action( instance.TestTimeSpanFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteField", new Action( instance.TestTinyByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteFieldArray", new Action( instance.TestTinyByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32Field", new Action( instance.TestTinyInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32FieldArray", new Action( instance.TestTinyInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64Field", new Action( instance.TestTinyInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64FieldArray", new Action( instance.TestTinyInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16Field", new Action( instance.TestTinyUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16FieldArray", new Action( instance.TestTinyUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_Complex", new Action( instance.TestToFromMessagePackObject_Complex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_ComplexGenerated", new Action( instance.TestToFromMessagePackObject_ComplexGenerated ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueField", new Action( instance.TestTrueField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueFieldArray", new Action( instance.TestTrueFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierDoesNotLoadTypeItself", new Action( instance.TestTypeVerifierDoesNotLoadTypeItself ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo", new Action( instance.TestUnpackTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUri", new Action( instance.TestUri ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriField", new Action( instance.TestUriField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArray", new Action( instance.TestUriFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArrayNull", new Action( instance.TestUriFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldNull", new Action( instance.TestUriFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestValueType_Success", new Action( instance.TestValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinor", new Action( instance.TestVersionConstructorMajorMinor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArray", new Action( instance.TestVersionConstructorMajorMinorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArrayNull", new Action( instance.TestVersionConstructorMajorMinorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuild", new Action( instance.TestVersionConstructorMajorMinorBuild ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArray", new Action( instance.TestVersionConstructorMajorMinorBuildArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArrayNull", new Action( instance.TestVersionConstructorMajorMinorBuildArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildNull", new Action( instance.TestVersionConstructorMajorMinorBuildNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorNull", new Action( instance.TestVersionConstructorMajorMinorNull ) ) );
		}
	} 

	internal static class ArrayReflectionBasedEnumSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new ArrayReflectionBasedEnumSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( ArrayReflectionBasedEnumSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByName", new Action( instance.TestEnumByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByName", new Action( instance.TestEnumByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByName", new Action( instance.TestEnumInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByName", new Action( instance.TestEnumInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByName", new Action( instance.TestEnumInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByName", new Action( instance.TestEnumInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByName", new Action( instance.TestEnumInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByName", new Action( instance.TestEnumInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_AllUpper", new Action( instance.TestEnumKeyTransformer_AllUpper ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Custom", new Action( instance.TestEnumKeyTransformer_Custom ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Default_AsIs", new Action( instance.TestEnumKeyTransformer_Default_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_LowerCamel", new Action( instance.TestEnumKeyTransformer_LowerCamel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByName", new Action( instance.TestEnumSByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByName", new Action( instance.TestEnumSByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByName", new Action( instance.TestEnumUInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByName", new Action( instance.TestEnumUInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByName", new Action( instance.TestEnumUInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByName", new Action( instance.TestEnumUInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByName", new Action( instance.TestEnumUInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByName", new Action( instance.TestEnumUInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsNone ) ) );
		}
	} 

	internal static class ArrayReflectionBasedReflectionMessagePackSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new ArrayReflectionBasedReflectionMessagePackSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( ArrayReflectionBasedReflectionMessagePackSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionKnownType_Success", new Action( instance.TestAbstractClassCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionNoAttribute_Success", new Action( instance.TestAbstractClassCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionRuntimeType_Success", new Action( instance.TestAbstractClassCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyKnownType_Success", new Action( instance.TestAbstractClassDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyNoAttribute_Fail", new Action( instance.TestAbstractClassDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyRuntimeType_Success", new Action( instance.TestAbstractClassDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemKnownType_Success", new Action( instance.TestAbstractClassListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemNoAttribute_Fail", new Action( instance.TestAbstractClassListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemRuntimeType_Success", new Action( instance.TestAbstractClassListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberKnownType_Success", new Action( instance.TestAbstractClassMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberNoAttribute_Fail", new Action( instance.TestAbstractClassMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberRuntimeType_Success", new Action( instance.TestAbstractClassMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_Default_Success", new Action( instance.TestAbstractTypes_KnownCollections_Default_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistration_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistration_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_WithoutRegistration_Fail", new Action( instance.TestAbstractTypes_KnownCollections_WithoutRegistration_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_NotACollection_Fail", new Action( instance.TestAbstractTypes_NotACollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeField", new Action( instance.TestAddOnlyCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArray", new Action( instance.TestAddOnlyCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArrayNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectField", new Action( instance.TestAddOnlyCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArray", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectField", new Action( instance.TestAddOnlyCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArray", new Action( instance.TestAddOnlyCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldNull", new Action( instance.TestAddOnlyCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListField", new Action( instance.TestArrayListField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListFieldArray", new Action( instance.TestArrayListFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListFieldArrayNull", new Action( instance.TestArrayListFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListFieldNull", new Action( instance.TestArrayListFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteField", new Action( instance.TestArraySegmentByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteFieldArray", new Action( instance.TestArraySegmentByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalField", new Action( instance.TestArraySegmentDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalFieldArray", new Action( instance.TestArraySegmentDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32Field", new Action( instance.TestArraySegmentInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32FieldArray", new Action( instance.TestArraySegmentInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoDefaultConstructor_Packable", new Action( instance.TestAsymmetric_PackOnly_NoDefaultConstructor_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable", new Action( instance.TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownCollectionItem_Fail", new Action( instance.TestAttribute_DuplicatedKnownCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownDictionaryKey_Fail", new Action( instance.TestAttribute_DuplicatedKnownDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownMember_Fail", new Action( instance.TestAttribute_DuplicatedKnownMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeCollectionItem_Fail", new Action( instance.TestAttribute_KnownAndRuntimeCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeDictionaryKey_Fail", new Action( instance.TestAttribute_KnownAndRuntimeDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeMember_Fail", new Action( instance.TestAttribute_KnownAndRuntimeMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBinary_ClassicContext", new Action( instance.TestBinary_ClassicContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBinary_ContextWithPackerCompatilibyOptionsNone", new Action( instance.TestBinary_ContextWithPackerCompatilibyOptionsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayContent", new Action( instance.TestByteArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayField", new Action( instance.TestByteArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArray", new Action( instance.TestByteArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArrayNull", new Action( instance.TestByteArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldNull", new Action( instance.TestByteArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteField", new Action( instance.TestByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteFieldArray", new Action( instance.TestByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayContent", new Action( instance.TestCharArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayField", new Action( instance.TestCharArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArray", new Action( instance.TestCharArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArrayNull", new Action( instance.TestCharArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldNull", new Action( instance.TestCharArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharField", new Action( instance.TestCharField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharFieldArray", new Action( instance.TestCharFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectField", new Action( instance.TestCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArray", new Action( instance.TestCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldNull", new Action( instance.TestCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_Aware", new Action( instance.TestCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_NotAware", new Action( instance.TestCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_Aware", new Action( instance.TestCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_NotAware", new Action( instance.TestCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Success", new Action( instance.TestCollection_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_Aware", new Action( instance.TestCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_NotAware", new Action( instance.TestCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeField", new Action( instance.TestCollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArray", new Action( instance.TestCollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArrayNull", new Action( instance.TestCollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldNull", new Action( instance.TestCollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectField", new Action( instance.TestCollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArray", new Action( instance.TestCollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArrayNull", new Action( instance.TestCollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldNull", new Action( instance.TestCollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObject_WithoutShortcut", new Action( instance.TestComplexObject_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObject_WithShortcut", new Action( instance.TestComplexObject_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithDataContract_WithoutShortcut", new Action( instance.TestComplexObjectTypeWithDataContract_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithDataContract_WithShortcut", new Action( instance.TestComplexObjectTypeWithDataContract_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithNonSerialized_WithoutShortcut", new Action( instance.TestComplexObjectTypeWithNonSerialized_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithNonSerialized_WithShortcut", new Action( instance.TestComplexObjectTypeWithNonSerialized_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGenerated_WithoutShortcut", new Action( instance.TestComplexTypeGenerated_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGenerated_WithShortcut", new Action( instance.TestComplexTypeGenerated_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedArray_WithoutShortcut", new Action( instance.TestComplexTypeGeneratedArray_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedArray_WithShortcut", new Action( instance.TestComplexTypeGeneratedArray_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosure_WithoutShortcut", new Action( instance.TestComplexTypeGeneratedEnclosure_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosure_WithShortcut", new Action( instance.TestComplexTypeGeneratedEnclosure_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosureArray_WithoutShortcut", new Action( instance.TestComplexTypeGeneratedEnclosureArray_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosureArray_WithShortcut", new Action( instance.TestComplexTypeGeneratedEnclosureArray_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithDataContractWithOrder_WithoutShortcut", new Action( instance.TestComplexTypeWithDataContractWithOrder_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithDataContractWithOrder_WithShortcut", new Action( instance.TestComplexTypeWithDataContractWithOrder_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithoutAnyAttribute_WithoutShortcut", new Action( instance.TestComplexTypeWithoutAnyAttribute_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithoutAnyAttribute_WithShortcut", new Action( instance.TestComplexTypeWithoutAnyAttribute_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstructorDeserializationWithParametersNotInLexicalOrder", new Action( instance.TestConstructorDeserializationWithParametersNotInLexicalOrder ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithAnotherNameConstrtor_DifferIsSetDefault", new Action( instance.TestConstrutorDeserializationWithAnotherNameConstrtor_DifferIsSetDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithAnotherTypeConstrtor_DifferIsSetDefault", new Action( instance.TestConstrutorDeserializationWithAnotherTypeConstrtor_DifferIsSetDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithAttribute_Preferred", new Action( instance.TestConstrutorDeserializationWithAttribute_Preferred ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithMultipleAttributes_Fail", new Action( instance.TestConstrutorDeserializationWithMultipleAttributes_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeNamedProperties", new Action( instance.TestDataMemberAttributeNamedProperties ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBase", new Action( instance.TestDataMemberAttributeOrderWithOneBase ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBase_ProtoBufCompatible", new Action( instance.TestDataMemberAttributeOrderWithOneBase_ProtoBufCompatible ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBaseDeserialize", new Action( instance.TestDataMemberAttributeOrderWithOneBaseDeserialize ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBaseDeserialize_ProtoBufCompatible", new Action( instance.TestDataMemberAttributeOrderWithOneBaseDeserialize_ProtoBufCompatible ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithZeroBase_ProtoBufCompatible_Fail", new Action( instance.TestDataMemberAttributeOrderWithZeroBase_ProtoBufCompatible_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTime", new Action( instance.TestDateTime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeClassic", new Action( instance.TestDateTimeClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeField", new Action( instance.TestDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeFieldArray", new Action( instance.TestDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Local", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Utc", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Local", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Utc", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeNullableChangeOnDemand", new Action( instance.TestDateTimeNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffset", new Action( instance.TestDateTimeOffset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetClassic", new Action( instance.TestDateTimeOffsetClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetField", new Action( instance.TestDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetFieldArray", new Action( instance.TestDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetNullableChangeOnDemand", new Action( instance.TestDateTimeOffsetNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalField", new Action( instance.TestDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalFieldArray", new Action( instance.TestDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_Aware", new Action( instance.TestDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_NotAware", new Action( instance.TestDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_Aware", new Action( instance.TestDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_NotAware", new Action( instance.TestDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_Aware", new Action( instance.TestDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_NotAware", new Action( instance.TestDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryField", new Action( instance.TestDictionaryEntryField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryFieldArray", new Action( instance.TestDictionaryEntryFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectField", new Action( instance.TestDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArray", new Action( instance.TestDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArrayNull", new Action( instance.TestDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldNull", new Action( instance.TestDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeField", new Action( instance.TestDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArray", new Action( instance.TestDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldNull", new Action( instance.TestDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes", new Action( instance.TestEmptyBytes ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes_Classic", new Action( instance.TestEmptyBytes_Classic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyIntArray", new Action( instance.TestEmptyIntArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyKeyValuePairArray", new Action( instance.TestEmptyKeyValuePairArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyMap", new Action( instance.TestEmptyMap ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyString", new Action( instance.TestEmptyString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnum", new Action( instance.TestEnum ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_Aware", new Action( instance.TestEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_NotAware", new Action( instance.TestEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_Aware", new Action( instance.TestEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_NotAware", new Action( instance.TestEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_Aware", new Action( instance.TestEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_NotAware", new Action( instance.TestEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExplicitlyImplementedPackableUnpackable", new Action( instance.TestExplicitlyImplementedPackableUnpackable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ClassicContext", new Action( instance.TestExt_ClassicContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ContextWithPackerCompatilibyOptionsNone", new Action( instance.TestExt_ContextWithPackerCompatilibyOptionsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_DefaultContext", new Action( instance.TestExt_DefaultContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseField", new Action( instance.TestFalseField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseFieldArray", new Action( instance.TestFalseFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEField", new Action( instance.TestFILETIMEField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEFieldArray", new Action( instance.TestFILETIMEFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructor", new Action( instance.TestFullVersionConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArray", new Action( instance.TestFullVersionConstructorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArrayNull", new Action( instance.TestFullVersionConstructorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorNull", new Action( instance.TestFullVersionConstructorNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetOnlyAndConstructor", new Action( instance.TestGetOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGlobalNamespace", new Action( instance.TestGlobalNamespace ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidField", new Action( instance.TestGuidField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidFieldArray", new Action( instance.TestGuidFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasGetOnlyPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithBothConstructor_Success", new Action( instance.TestHasGetOnlyPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithDefaultConstructor_Fail", new Action( instance.TestHasGetOnlyPropertyWithDefaultConstructor_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithRecordConstructor_Success", new Action( instance.TestHasGetOnlyPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectField", new Action( instance.TestHashSet_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArray", new Action( instance.TestHashSet_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArrayNull", new Action( instance.TestHashSet_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldNull", new Action( instance.TestHashSet_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeField", new Action( instance.TestHashSetDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArray", new Action( instance.TestHashSetDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArrayNull", new Action( instance.TestHashSetDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldNull", new Action( instance.TestHashSetDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectField", new Action( instance.TestHashSetObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArray", new Action( instance.TestHashSetObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArrayNull", new Action( instance.TestHashSetObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldNull", new Action( instance.TestHashSetObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableField", new Action( instance.TestHashtableField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableFieldArray", new Action( instance.TestHashtableFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableFieldArrayNull", new Action( instance.TestHashtableFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableFieldNull", new Action( instance.TestHashtableFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasInitOnlyFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithBothConstructor_Success", new Action( instance.TestHasInitOnlyFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithDefaultConstructor_Fail", new Action( instance.TestHasInitOnlyFieldWithDefaultConstructor_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithRecordConstructor_Success", new Action( instance.TestHasInitOnlyFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithBothConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithRecordConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithBothConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithRecordConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithBothConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasReadWriteFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithBothConstructor_Success", new Action( instance.TestHasReadWriteFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithDefaultConstructor_Success", new Action( instance.TestHasReadWriteFieldWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithRecordConstructor_Success", new Action( instance.TestHasReadWriteFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectField", new Action( instance.TestICollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArray", new Action( instance.TestICollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArrayNull", new Action( instance.TestICollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldNull", new Action( instance.TestICollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeField", new Action( instance.TestICollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArray", new Action( instance.TestICollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArrayNull", new Action( instance.TestICollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldNull", new Action( instance.TestICollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectField", new Action( instance.TestICollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArray", new Action( instance.TestICollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArrayNull", new Action( instance.TestICollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldNull", new Action( instance.TestICollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectField", new Action( instance.TestIDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArray", new Action( instance.TestIDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArrayNull", new Action( instance.TestIDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldNull", new Action( instance.TestIDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeField", new Action( instance.TestIDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArray", new Action( instance.TestIDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestIDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldNull", new Action( instance.TestIDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryValueType_Success", new Action( instance.TestIDictionaryValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_ExcludedOnly", new Action( instance.TestIgnore_ExcludedOnly ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_ExclusionAndInclusionMixed", new Action( instance.TestIgnore_ExclusionAndInclusionMixed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_ExclusionAndInclusionSimulatously", new Action( instance.TestIgnore_ExclusionAndInclusionSimulatously ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_Normal", new Action( instance.TestIgnore_Normal ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectField", new Action( instance.TestIList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArray", new Action( instance.TestIList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArrayNull", new Action( instance.TestIList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldNull", new Action( instance.TestIList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeField", new Action( instance.TestIListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArray", new Action( instance.TestIListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArrayNull", new Action( instance.TestIListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldNull", new Action( instance.TestIListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectField", new Action( instance.TestIListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArray", new Action( instance.TestIListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArrayNull", new Action( instance.TestIListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldNull", new Action( instance.TestIListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListValueType_Success", new Action( instance.TestIListValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_Field", new Action( instance.TestImage_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArray", new Action( instance.TestImage_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArrayNull", new Action( instance.TestImage_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldNull", new Action( instance.TestImage_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail", new Action( instance.TestImplementsGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsNonGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail", new Action( instance.TestImplementsNonGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsNonGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsNonGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32", new Action( instance.TestInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64", new Action( instance.TestInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionKnownType_Success", new Action( instance.TestInterfaceCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionNoAttribute_Success", new Action( instance.TestInterfaceCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionRuntimeType_Success", new Action( instance.TestInterfaceCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyKnownType_Success", new Action( instance.TestInterfaceDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyNoAttribute_Fail", new Action( instance.TestInterfaceDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyRuntimeType_Success", new Action( instance.TestInterfaceDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemKnownType_Success", new Action( instance.TestInterfaceListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemNoAttribute_Fail", new Action( instance.TestInterfaceListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemRuntimeType_Success", new Action( instance.TestInterfaceListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberKnownType_Success", new Action( instance.TestInterfaceMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberNoAttribute_Fail", new Action( instance.TestInterfaceMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberRuntimeType_Success", new Action( instance.TestInterfaceMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_Plain", new Action( instance.TestIssue25_Plain ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_SelfComposite", new Action( instance.TestIssue25_SelfComposite ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetField", new Action( instance.TestKeyValuePairStringDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetFieldArray", new Action( instance.TestKeyValuePairStringDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Field_Known", new Action( instance.TestKnownType_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Property_Known", new Action( instance.TestKnownType_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Field_Known", new Action( instance.TestKnownType_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Property_Known", new Action( instance.TestKnownType_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectField", new Action( instance.TestList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArray", new Action( instance.TestList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArrayNull", new Action( instance.TestList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldNull", new Action( instance.TestList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_Aware", new Action( instance.TestList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_NotAware", new Action( instance.TestList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_Aware", new Action( instance.TestList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_NotAware", new Action( instance.TestList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_Aware", new Action( instance.TestList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_NotAware", new Action( instance.TestList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeField", new Action( instance.TestListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArray", new Action( instance.TestListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArrayNull", new Action( instance.TestListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldNull", new Action( instance.TestListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectField", new Action( instance.TestListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArray", new Action( instance.TestListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArrayNull", new Action( instance.TestListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldNull", new Action( instance.TestListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestManyMembers", new Action( instance.TestManyMembers ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteField", new Action( instance.TestMaxByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteFieldArray", new Action( instance.TestMaxByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32Field", new Action( instance.TestMaxInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32FieldArray", new Action( instance.TestMaxInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64Field", new Action( instance.TestMaxInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64FieldArray", new Action( instance.TestMaxInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16Field", new Action( instance.TestMaxUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16FieldArray", new Action( instance.TestMaxUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_Field", new Action( instance.TestMessagePackObject_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArray", new Action( instance.TestMessagePackObject_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArrayNull", new Action( instance.TestMessagePackObject_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldNull", new Action( instance.TestMessagePackObject_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_Field", new Action( instance.TestMessagePackObjectArray_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArray", new Action( instance.TestMessagePackObjectArray_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArrayNull", new Action( instance.TestMessagePackObjectArray_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldNull", new Action( instance.TestMessagePackObjectArray_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32Field", new Action( instance.TestMinInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32FieldArray", new Action( instance.TestMinInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64Field", new Action( instance.TestMinInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64FieldArray", new Action( instance.TestMinInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArray", new Action( instance.TestMultiDimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArrayComprex", new Action( instance.TestMultiDimensionalArrayComprex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection", new Action( instance.TestNameValueCollection ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection_NullKey", new Action( instance.TestNameValueCollection_NullKey ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_Fail", new Action( instance.TestNoMembers_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableFail", new Action( instance.TestNoMembers_PackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableUnpackableSuccess", new Action( instance.TestNoMembers_PackableUnpackableSuccess ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_UnpackableFail", new Action( instance.TestNoMembers_UnpackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_Aware", new Action( instance.TestNonGenericCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_NotAware", new Action( instance.TestNonGenericCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_Aware", new Action( instance.TestNonGenericCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_NotAware", new Action( instance.TestNonGenericCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_Aware", new Action( instance.TestNonGenericCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_NotAware", new Action( instance.TestNonGenericCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_Aware", new Action( instance.TestNonGenericDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_NotAware", new Action( instance.TestNonGenericDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_Aware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_NotAware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_Aware", new Action( instance.TestNonGenericDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_NotAware", new Action( instance.TestNonGenericDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_Aware", new Action( instance.TestNonGenericEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_NotAware", new Action( instance.TestNonGenericEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_Aware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_NotAware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_Aware", new Action( instance.TestNonGenericEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_NotAware", new Action( instance.TestNonGenericEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_Aware", new Action( instance.TestNonGenericList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_NotAware", new Action( instance.TestNonGenericList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_Aware", new Action( instance.TestNonGenericList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_NotAware", new Action( instance.TestNonGenericList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_Aware", new Action( instance.TestNonGenericList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_NotAware", new Action( instance.TestNonGenericList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_DataContract_Failed", new Action( instance.TestNonPublicType_DataContract_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_MessagePackMember_Failed", new Action( instance.TestNonPublicType_MessagePackMember_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_Plain_Failed", new Action( instance.TestNonPublicType_Plain_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_DataContract", new Action( instance.TestNonPublicWritableMember_DataContract ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_MessagePackMember", new Action( instance.TestNonPublicWritableMember_MessagePackMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_PlainOldCliClass", new Action( instance.TestNonPublicWritableMember_PlainOldCliClass ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonZeroBoundMultidimensionalArray", new Action( instance.TestNonZeroBoundMultidimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullable", new Action( instance.TestNullable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullField", new Action( instance.TestNullField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArray", new Action( instance.TestNullFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArrayNull", new Action( instance.TestNullFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldNull", new Action( instance.TestNullFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayField", new Action( instance.TestObjectArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArray", new Action( instance.TestObjectArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArrayNull", new Action( instance.TestObjectArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldNull", new Action( instance.TestObjectArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectField", new Action( instance.TestObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArray", new Action( instance.TestObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArrayNull", new Action( instance.TestObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldNull", new Action( instance.TestObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorBoolean_Success", new Action( instance.TestOptionalConstructorBoolean_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorByte_Success", new Action( instance.TestOptionalConstructorByte_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorChar_Success", new Action( instance.TestOptionalConstructorChar_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorDecimal_Success", new Action( instance.TestOptionalConstructorDecimal_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorDouble_Success", new Action( instance.TestOptionalConstructorDouble_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorInt16_Success", new Action( instance.TestOptionalConstructorInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorInt32_Success", new Action( instance.TestOptionalConstructorInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorInt64_Success", new Action( instance.TestOptionalConstructorInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorSByte_Success", new Action( instance.TestOptionalConstructorSByte_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorSingle_Success", new Action( instance.TestOptionalConstructorSingle_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorString_Success", new Action( instance.TestOptionalConstructorString_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorUInt16_Success", new Action( instance.TestOptionalConstructorUInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorUInt32_Success", new Action( instance.TestOptionalConstructorUInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorUInt64_Success", new Action( instance.TestOptionalConstructorUInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackable_PackToMessageUsed", new Action( instance.TestPackable_PackToMessageUsed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackableUnpackable_PackToMessageAndUnpackFromMessageUsed", new Action( instance.TestPackableUnpackable_PackToMessageAndUnpackFromMessageUsed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Null_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphismAttributesInType", new Action( instance.TestPolymorphismAttributesInType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestReadOnlyAndConstructor", new Action( instance.TestReadOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStaticMembersDoNotCausePrepareError", new Action( instance.TestStaticMembersDoNotCausePrepareError ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestString", new Action( instance.TestString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringField", new Action( instance.TestStringField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArray", new Action( instance.TestStringFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArrayNull", new Action( instance.TestStringFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldNull", new Action( instance.TestStringFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeField", new Action( instance.TestStringKeyedCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArray", new Action( instance.TestStringKeyedCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArrayNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectField", new Action( instance.TestStringKeyedCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArray", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectField", new Action( instance.TestStringKeyedCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArray", new Action( instance.TestStringKeyedCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldNull", new Action( instance.TestStringKeyedCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanField", new Action( instance.TestTimeSpanField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanFieldArray", new Action( instance.TestTimeSpanFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteField", new Action( instance.TestTinyByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteFieldArray", new Action( instance.TestTinyByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32Field", new Action( instance.TestTinyInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32FieldArray", new Action( instance.TestTinyInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64Field", new Action( instance.TestTinyInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64FieldArray", new Action( instance.TestTinyInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16Field", new Action( instance.TestTinyUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16FieldArray", new Action( instance.TestTinyUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_Complex", new Action( instance.TestToFromMessagePackObject_Complex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_ComplexGenerated", new Action( instance.TestToFromMessagePackObject_ComplexGenerated ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueField", new Action( instance.TestTrueField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueFieldArray", new Action( instance.TestTrueFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierDoesNotLoadTypeItself", new Action( instance.TestTypeVerifierDoesNotLoadTypeItself ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeWithDuplicatedMessagePackMemberAttributeMember", new Action( instance.TestTypeWithDuplicatedMessagePackMemberAttributeMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeWithInvalidMessagePackMemberAttributeMember", new Action( instance.TestTypeWithInvalidMessagePackMemberAttributeMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeWithMissingMessagePackMemberAttributeMember", new Action( instance.TestTypeWithMissingMessagePackMemberAttributeMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackable_UnpackFromMessageUsed", new Action( instance.TestUnpackable_UnpackFromMessageUsed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo", new Action( instance.TestUnpackTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUri", new Action( instance.TestUri ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriField", new Action( instance.TestUriField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArray", new Action( instance.TestUriFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArrayNull", new Action( instance.TestUriFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldNull", new Action( instance.TestUriFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestValueType_Success", new Action( instance.TestValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinor", new Action( instance.TestVersionConstructorMajorMinor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArray", new Action( instance.TestVersionConstructorMajorMinorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArrayNull", new Action( instance.TestVersionConstructorMajorMinorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuild", new Action( instance.TestVersionConstructorMajorMinorBuild ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArray", new Action( instance.TestVersionConstructorMajorMinorBuildArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArrayNull", new Action( instance.TestVersionConstructorMajorMinorBuildArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildNull", new Action( instance.TestVersionConstructorMajorMinorBuildNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorNull", new Action( instance.TestVersionConstructorMajorMinorNull ) ) );
		}
	} 

	internal static class BigEndianBinaryTestInitializer
	{
		public static object CreateInstance()
		{
			return new BigEndianBinaryTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( BigEndianBinaryTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToByte", new Action( instance.TestToByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDouble", new Action( instance.TestToDouble ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToInt16", new Action( instance.TestToInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToInt32", new Action( instance.TestToInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToInt64", new Action( instance.TestToInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToSByte", new Action( instance.TestToSByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToSingle", new Action( instance.TestToSingle ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToUInt16", new Action( instance.TestToUInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToUInt32", new Action( instance.TestToUInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToUInt64", new Action( instance.TestToUInt64 ) ) );
		}
	} 

	internal static class ByteArrayPackerTestInitializer
	{
		public static object CreateInstance()
		{
			return new ByteArrayPackerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( ByteArrayPackerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Custom_Binary_EnoughSize",
					() => {
						instance.TestSingleAllocation_Custom_Binary_EnoughSize( 0 );
						instance.TestSingleAllocation_Custom_Binary_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Custom_Binary_TooShortSize",
					() => {
						instance.TestSingleAllocation_Custom_Binary_TooShortSize( 0 );
						instance.TestSingleAllocation_Custom_Binary_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Custom_Scalar_EnoughSize",
					() => {
						instance.TestSingleAllocation_Custom_Scalar_EnoughSize( 0 );
						instance.TestSingleAllocation_Custom_Scalar_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Custom_Scalar_TooShortSize",
					() => {
						instance.TestSingleAllocation_Custom_Scalar_TooShortSize( 0 );
						instance.TestSingleAllocation_Custom_Scalar_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Custom_String_EnoughSize",
					() => {
						instance.TestSingleAllocation_Custom_String_EnoughSize( 0 );
						instance.TestSingleAllocation_Custom_String_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Custom_String_TooShortSize",
					() => {
						instance.TestSingleAllocation_Custom_String_TooShortSize( 0 );
						instance.TestSingleAllocation_Custom_String_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Default_Binary_EnoughSize",
					() => {
						instance.TestSingleAllocation_Default_Binary_EnoughSize( 0 );
						instance.TestSingleAllocation_Default_Binary_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Default_Binary_TooShortSize",
					() => {
						instance.TestSingleAllocation_Default_Binary_TooShortSize( 0 );
						instance.TestSingleAllocation_Default_Binary_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Default_Scalar_EnoughSize",
					() => {
						instance.TestSingleAllocation_Default_Scalar_EnoughSize( 0 );
						instance.TestSingleAllocation_Default_Scalar_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Default_Scalar_TooShortSize",
					() => {
						instance.TestSingleAllocation_Default_Scalar_TooShortSize( 0 );
						instance.TestSingleAllocation_Default_Scalar_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Default_String_EnoughSize",
					() => {
						instance.TestSingleAllocation_Default_String_EnoughSize( 0 );
						instance.TestSingleAllocation_Default_String_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Default_String_TooShortSize",
					() => {
						instance.TestSingleAllocation_Default_String_TooShortSize( 0 );
						instance.TestSingleAllocation_Default_String_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Fixed_Binary_EnoughSize",
					() => {
						instance.TestSingleAllocation_Fixed_Binary_EnoughSize( 0 );
						instance.TestSingleAllocation_Fixed_Binary_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Fixed_Binary_TooShortSize",
					() => {
						instance.TestSingleAllocation_Fixed_Binary_TooShortSize( 0 );
						instance.TestSingleAllocation_Fixed_Binary_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Fixed_Scalar_EnoughSize",
					() => {
						instance.TestSingleAllocation_Fixed_Scalar_EnoughSize( 0 );
						instance.TestSingleAllocation_Fixed_Scalar_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Fixed_Scalar_TooShortSize",
					() => {
						instance.TestSingleAllocation_Fixed_Scalar_TooShortSize( 0 );
						instance.TestSingleAllocation_Fixed_Scalar_TooShortSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Fixed_String_EnoughSize",
					() => {
						instance.TestSingleAllocation_Fixed_String_EnoughSize( 0 );
						instance.TestSingleAllocation_Fixed_String_EnoughSize( 1 );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestSingleAllocation_Fixed_String_TooShortSize",
					() => {
						instance.TestSingleAllocation_Fixed_String_TooShortSize( 0 );
						instance.TestSingleAllocation_Fixed_String_TooShortSize( 1 );
					}
				)
			);
		}
	} 

	internal static class DirectConversionTestInitializer
	{
		public static object CreateInstance()
		{
			return new DirectConversionTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( DirectConversionTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArray", new Action( instance.TestArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBoolean", new Action( instance.TestBoolean ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByte", new Action( instance.TestByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDouble", new Action( instance.TestDouble ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt16", new Action( instance.TestInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32", new Action( instance.TestInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64", new Action( instance.TestInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMap", new Action( instance.TestMap ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNil", new Action( instance.TestNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSByte", new Action( instance.TestSByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSingle", new Action( instance.TestSingle ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestString", new Action( instance.TestString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt16", new Action( instance.TestUInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt32", new Action( instance.TestUInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt64", new Action( instance.TestUInt64 ) ) );
		}
	} 

	internal static class EqualsTestInitializer
	{
		public static object CreateInstance()
		{
			return new EqualsTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( EqualsTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt", new Action( instance.TestInt ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestLong", new Action( instance.TestLong ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNil", new Action( instance.TestNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestString", new Action( instance.TestString ) ) );
		}
	} 

	internal static class GenerationBasedNilImplicationTestInitializer
	{
		public static object CreateInstance()
		{
			return new GenerationBasedNilImplicationTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( GenerationBasedNilImplicationTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_NullableValueType_AllOk", new Action( instance.TestCreation_NullableValueType_AllOk ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_ReadOnlyCollectionProperty_OnlyNullIsInvalid", new Action( instance.TestCreation_ReadOnlyCollectionProperty_OnlyNullIsInvalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_ReferenceType_AllOk", new Action( instance.TestCreation_ReferenceType_AllOk ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_ValueType_OnlyNullIsInvalid", new Action( instance.TestCreation_ValueType_OnlyNullIsInvalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestElelementMissingInTheFirstPlace_Map_MissingMembersAreSkipped", new Action( instance.TestElelementMissingInTheFirstPlace_Map_MissingMembersAreSkipped ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestElelementTooManyInTheFirstPlace_Map_ExtrasAreIgnored", new Action( instance.TestElelementTooManyInTheFirstPlace_Map_ExtrasAreIgnored ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_NullableValueType_ProhibitWillFail_Array", new Action( instance.TestPack_NullableValueType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_NullableValueType_ProhibitWillFail_Map", new Action( instance.TestPack_NullableValueType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Array", new Action( instance.TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Map", new Action( instance.TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReferenceType_ProhibitWillFail_Array", new Action( instance.TestPack_ReferenceType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReferenceType_ProhibitWillFail_Map", new Action( instance.TestPack_ReferenceType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Array", new Action( instance.TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Map", new Action( instance.TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_NullWillBeNull_Array", new Action( instance.TestUnpack_NullableValueType_NullWillBeNull_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_NullWillBeNull_Map", new Action( instance.TestUnpack_NullableValueType_NullWillBeNull_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_ProhibitWillFail_Array", new Action( instance.TestUnpack_NullableValueType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_ProhibitWillFail_Map", new Action( instance.TestUnpack_NullableValueType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Array", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Map", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Array", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Map", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Array", new Action( instance.TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Map", new Action( instance.TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_NullWillBeNull_Array", new Action( instance.TestUnpack_ReferenceType_NullWillBeNull_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_NullWillBeNull_Map", new Action( instance.TestUnpack_ReferenceType_NullWillBeNull_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_ProhibitWillFail_Array", new Action( instance.TestUnpack_ReferenceType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_ProhibitWillFail_Map", new Action( instance.TestUnpack_ReferenceType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_MemberDefault_Preserved_Array", new Action( instance.TestUnpack_ValueType_MemberDefault_Preserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_MemberDefault_Preserved_Map", new Action( instance.TestUnpack_ValueType_MemberDefault_Preserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_Prohibit_Fail_Array", new Action( instance.TestUnpack_ValueType_Prohibit_Fail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_Prohibit_Fail_Map", new Action( instance.TestUnpack_ValueType_Prohibit_Fail_Map ) ) );
		}
	} 

	internal static class InheritanceTestInitializer
	{
		public static object CreateInstance()
		{
			return new InheritanceTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( InheritanceTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "Issue147_161_162_HidingMembersWillNotBeDuplicated", new Action( instance.Issue147_161_162_HidingMembersWillNotBeDuplicated ) ) );
		}
	} 

	internal static class KeyNameTransformersTestInitializer
	{
		public static object CreateInstance()
		{
			return new KeyNameTransformersTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( KeyNameTransformersTest )testFixtureInstance );
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestToLowerCamel",
					() => {
						instance.TestToLowerCamel( null, null );
						instance.TestToLowerCamel( @"", @"" );
						instance.TestToLowerCamel( @"a", @"a" );
						instance.TestToLowerCamel( @"A", @"a" );
						instance.TestToLowerCamel( @"AA", @"aA" );
						instance.TestToLowerCamel( @"Aa", @"aa" );
						instance.TestToLowerCamel( @"aa", @"aa" );
						instance.TestToLowerCamel( @"_a", @"_a" );
						instance.TestToLowerCamel( @"_A", @"_A" );
					}
				)
			);
			testClassInstance.TestMethods.Add(
				new TestMethod(
					"TestToUpperSnake",
					() => {
						instance.TestToUpperSnake( null, null );
						instance.TestToUpperSnake( @"", @"" );
						instance.TestToUpperSnake( @"a", @"A" );
						instance.TestToUpperSnake( @"A", @"A" );
						instance.TestToUpperSnake( @"AA", @"AA" );
						instance.TestToUpperSnake( @"Aa", @"AA" );
						instance.TestToUpperSnake( @"aa", @"AA" );
						instance.TestToUpperSnake( @"aA", @"A_A" );
						instance.TestToUpperSnake( @"AAa", @"AAA" );
						instance.TestToUpperSnake( @"AaA", @"AA_A" );
						instance.TestToUpperSnake( @"aaA", @"AA_A" );
						instance.TestToUpperSnake( @"_a", @"_A" );
						instance.TestToUpperSnake( @"_A", @"_A" );
					}
				)
			);
		}
	} 

	internal static class MapGenerationBasedEnumSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new MapGenerationBasedEnumSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MapGenerationBasedEnumSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByName", new Action( instance.TestEnumByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByName", new Action( instance.TestEnumByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByName", new Action( instance.TestEnumInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByName", new Action( instance.TestEnumInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByName", new Action( instance.TestEnumInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByName", new Action( instance.TestEnumInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByName", new Action( instance.TestEnumInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByName", new Action( instance.TestEnumInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_AllUpper", new Action( instance.TestEnumKeyTransformer_AllUpper ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Custom", new Action( instance.TestEnumKeyTransformer_Custom ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Default_AsIs", new Action( instance.TestEnumKeyTransformer_Default_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_LowerCamel", new Action( instance.TestEnumKeyTransformer_LowerCamel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByName", new Action( instance.TestEnumSByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByName", new Action( instance.TestEnumSByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByName", new Action( instance.TestEnumUInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByName", new Action( instance.TestEnumUInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByName", new Action( instance.TestEnumUInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByName", new Action( instance.TestEnumUInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByName", new Action( instance.TestEnumUInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByName", new Action( instance.TestEnumUInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue ) ) );
		}
	} 

	internal static class MapGenerationBasedReflectionMessagePackSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new MapGenerationBasedReflectionMessagePackSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MapGenerationBasedReflectionMessagePackSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionKnownType_Success", new Action( instance.TestAbstractClassCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionNoAttribute_Success", new Action( instance.TestAbstractClassCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionRuntimeType_Success", new Action( instance.TestAbstractClassCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyKnownType_Success", new Action( instance.TestAbstractClassDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyNoAttribute_Fail", new Action( instance.TestAbstractClassDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyRuntimeType_Success", new Action( instance.TestAbstractClassDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemKnownType_Success", new Action( instance.TestAbstractClassListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemNoAttribute_Fail", new Action( instance.TestAbstractClassListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemRuntimeType_Success", new Action( instance.TestAbstractClassListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberKnownType_Success", new Action( instance.TestAbstractClassMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberNoAttribute_Fail", new Action( instance.TestAbstractClassMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberRuntimeType_Success", new Action( instance.TestAbstractClassMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_Default_Success", new Action( instance.TestAbstractTypes_KnownCollections_Default_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistration_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistration_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_WithoutRegistration_Fail", new Action( instance.TestAbstractTypes_KnownCollections_WithoutRegistration_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_NotACollection_Fail", new Action( instance.TestAbstractTypes_NotACollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeField", new Action( instance.TestAddOnlyCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArray", new Action( instance.TestAddOnlyCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArrayNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectField", new Action( instance.TestAddOnlyCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArray", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectField", new Action( instance.TestAddOnlyCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArray", new Action( instance.TestAddOnlyCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldNull", new Action( instance.TestAddOnlyCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteField", new Action( instance.TestArraySegmentByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteFieldArray", new Action( instance.TestArraySegmentByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalField", new Action( instance.TestArraySegmentDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalFieldArray", new Action( instance.TestArraySegmentDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32Field", new Action( instance.TestArraySegmentInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32FieldArray", new Action( instance.TestArraySegmentInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoDefaultConstructor_Packable", new Action( instance.TestAsymmetric_PackOnly_NoDefaultConstructor_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable", new Action( instance.TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownCollectionItem_Fail", new Action( instance.TestAttribute_DuplicatedKnownCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownDictionaryKey_Fail", new Action( instance.TestAttribute_DuplicatedKnownDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownMember_Fail", new Action( instance.TestAttribute_DuplicatedKnownMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeCollectionItem_Fail", new Action( instance.TestAttribute_KnownAndRuntimeCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeDictionaryKey_Fail", new Action( instance.TestAttribute_KnownAndRuntimeDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeMember_Fail", new Action( instance.TestAttribute_KnownAndRuntimeMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayContent", new Action( instance.TestByteArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayField", new Action( instance.TestByteArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArray", new Action( instance.TestByteArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArrayNull", new Action( instance.TestByteArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldNull", new Action( instance.TestByteArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteField", new Action( instance.TestByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteFieldArray", new Action( instance.TestByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayContent", new Action( instance.TestCharArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayField", new Action( instance.TestCharArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArray", new Action( instance.TestCharArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArrayNull", new Action( instance.TestCharArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldNull", new Action( instance.TestCharArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharField", new Action( instance.TestCharField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharFieldArray", new Action( instance.TestCharFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectField", new Action( instance.TestCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArray", new Action( instance.TestCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldNull", new Action( instance.TestCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_Aware", new Action( instance.TestCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_NotAware", new Action( instance.TestCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_Aware", new Action( instance.TestCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_NotAware", new Action( instance.TestCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Success", new Action( instance.TestCollection_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_Aware", new Action( instance.TestCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_NotAware", new Action( instance.TestCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeField", new Action( instance.TestCollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArray", new Action( instance.TestCollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArrayNull", new Action( instance.TestCollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldNull", new Action( instance.TestCollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectField", new Action( instance.TestCollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArray", new Action( instance.TestCollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArrayNull", new Action( instance.TestCollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldNull", new Action( instance.TestCollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstructorDeserializationWithParametersNotInLexicalOrder", new Action( instance.TestConstructorDeserializationWithParametersNotInLexicalOrder ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTime", new Action( instance.TestDateTime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeClassic", new Action( instance.TestDateTimeClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeField", new Action( instance.TestDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeFieldArray", new Action( instance.TestDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Local", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Utc", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Local", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Utc", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeNullableChangeOnDemand", new Action( instance.TestDateTimeNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffset", new Action( instance.TestDateTimeOffset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetClassic", new Action( instance.TestDateTimeOffsetClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetField", new Action( instance.TestDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetFieldArray", new Action( instance.TestDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetNullableChangeOnDemand", new Action( instance.TestDateTimeOffsetNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalField", new Action( instance.TestDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalFieldArray", new Action( instance.TestDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_Aware", new Action( instance.TestDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_NotAware", new Action( instance.TestDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_Aware", new Action( instance.TestDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_NotAware", new Action( instance.TestDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_Aware", new Action( instance.TestDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_NotAware", new Action( instance.TestDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryField", new Action( instance.TestDictionaryEntryField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryFieldArray", new Action( instance.TestDictionaryEntryFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectField", new Action( instance.TestDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArray", new Action( instance.TestDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArrayNull", new Action( instance.TestDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldNull", new Action( instance.TestDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeField", new Action( instance.TestDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArray", new Action( instance.TestDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldNull", new Action( instance.TestDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes", new Action( instance.TestEmptyBytes ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes_Classic", new Action( instance.TestEmptyBytes_Classic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyIntArray", new Action( instance.TestEmptyIntArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyKeyValuePairArray", new Action( instance.TestEmptyKeyValuePairArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyMap", new Action( instance.TestEmptyMap ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyString", new Action( instance.TestEmptyString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnum", new Action( instance.TestEnum ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_Aware", new Action( instance.TestEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_NotAware", new Action( instance.TestEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_Aware", new Action( instance.TestEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_NotAware", new Action( instance.TestEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_Aware", new Action( instance.TestEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_NotAware", new Action( instance.TestEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExplicitlyImplementedPackableUnpackable", new Action( instance.TestExplicitlyImplementedPackableUnpackable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ClassicContext", new Action( instance.TestExt_ClassicContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ContextWithPackerCompatilibyOptionsNone", new Action( instance.TestExt_ContextWithPackerCompatilibyOptionsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_DefaultContext", new Action( instance.TestExt_DefaultContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseField", new Action( instance.TestFalseField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseFieldArray", new Action( instance.TestFalseFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEField", new Action( instance.TestFILETIMEField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEFieldArray", new Action( instance.TestFILETIMEFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructor", new Action( instance.TestFullVersionConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArray", new Action( instance.TestFullVersionConstructorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArrayNull", new Action( instance.TestFullVersionConstructorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorNull", new Action( instance.TestFullVersionConstructorNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetOnlyAndConstructor", new Action( instance.TestGetOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGlobalNamespace", new Action( instance.TestGlobalNamespace ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidField", new Action( instance.TestGuidField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidFieldArray", new Action( instance.TestGuidFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectField", new Action( instance.TestHashSet_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArray", new Action( instance.TestHashSet_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArrayNull", new Action( instance.TestHashSet_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldNull", new Action( instance.TestHashSet_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeField", new Action( instance.TestHashSetDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArray", new Action( instance.TestHashSetDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArrayNull", new Action( instance.TestHashSetDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldNull", new Action( instance.TestHashSetDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectField", new Action( instance.TestHashSetObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArray", new Action( instance.TestHashSetObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArrayNull", new Action( instance.TestHashSetObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldNull", new Action( instance.TestHashSetObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectField", new Action( instance.TestICollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArray", new Action( instance.TestICollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArrayNull", new Action( instance.TestICollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldNull", new Action( instance.TestICollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeField", new Action( instance.TestICollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArray", new Action( instance.TestICollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArrayNull", new Action( instance.TestICollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldNull", new Action( instance.TestICollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectField", new Action( instance.TestICollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArray", new Action( instance.TestICollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArrayNull", new Action( instance.TestICollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldNull", new Action( instance.TestICollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectField", new Action( instance.TestIDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArray", new Action( instance.TestIDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArrayNull", new Action( instance.TestIDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldNull", new Action( instance.TestIDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeField", new Action( instance.TestIDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArray", new Action( instance.TestIDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestIDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldNull", new Action( instance.TestIDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryValueType_Success", new Action( instance.TestIDictionaryValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectField", new Action( instance.TestIList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArray", new Action( instance.TestIList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArrayNull", new Action( instance.TestIList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldNull", new Action( instance.TestIList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeField", new Action( instance.TestIListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArray", new Action( instance.TestIListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArrayNull", new Action( instance.TestIListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldNull", new Action( instance.TestIListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectField", new Action( instance.TestIListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArray", new Action( instance.TestIListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArrayNull", new Action( instance.TestIListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldNull", new Action( instance.TestIListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListValueType_Success", new Action( instance.TestIListValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_Field", new Action( instance.TestImage_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArray", new Action( instance.TestImage_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArrayNull", new Action( instance.TestImage_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldNull", new Action( instance.TestImage_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsNonGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsNonGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32", new Action( instance.TestInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64", new Action( instance.TestInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionKnownType_Success", new Action( instance.TestInterfaceCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionNoAttribute_Success", new Action( instance.TestInterfaceCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionRuntimeType_Success", new Action( instance.TestInterfaceCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyKnownType_Success", new Action( instance.TestInterfaceDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyNoAttribute_Fail", new Action( instance.TestInterfaceDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyRuntimeType_Success", new Action( instance.TestInterfaceDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemKnownType_Success", new Action( instance.TestInterfaceListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemNoAttribute_Fail", new Action( instance.TestInterfaceListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemRuntimeType_Success", new Action( instance.TestInterfaceListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberKnownType_Success", new Action( instance.TestInterfaceMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberNoAttribute_Fail", new Action( instance.TestInterfaceMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberRuntimeType_Success", new Action( instance.TestInterfaceMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_Plain", new Action( instance.TestIssue25_Plain ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_SelfComposite", new Action( instance.TestIssue25_SelfComposite ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetField", new Action( instance.TestKeyValuePairStringDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetFieldArray", new Action( instance.TestKeyValuePairStringDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Field_Known", new Action( instance.TestKnownType_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Property_Known", new Action( instance.TestKnownType_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Field_Known", new Action( instance.TestKnownType_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Property_Known", new Action( instance.TestKnownType_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectField", new Action( instance.TestList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArray", new Action( instance.TestList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArrayNull", new Action( instance.TestList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldNull", new Action( instance.TestList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_Aware", new Action( instance.TestList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_NotAware", new Action( instance.TestList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_Aware", new Action( instance.TestList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_NotAware", new Action( instance.TestList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_Aware", new Action( instance.TestList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_NotAware", new Action( instance.TestList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeField", new Action( instance.TestListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArray", new Action( instance.TestListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArrayNull", new Action( instance.TestListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldNull", new Action( instance.TestListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectField", new Action( instance.TestListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArray", new Action( instance.TestListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArrayNull", new Action( instance.TestListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldNull", new Action( instance.TestListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestManyMembers", new Action( instance.TestManyMembers ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteField", new Action( instance.TestMaxByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteFieldArray", new Action( instance.TestMaxByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32Field", new Action( instance.TestMaxInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32FieldArray", new Action( instance.TestMaxInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64Field", new Action( instance.TestMaxInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64FieldArray", new Action( instance.TestMaxInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16Field", new Action( instance.TestMaxUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16FieldArray", new Action( instance.TestMaxUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_Field", new Action( instance.TestMessagePackObject_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArray", new Action( instance.TestMessagePackObject_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArrayNull", new Action( instance.TestMessagePackObject_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldNull", new Action( instance.TestMessagePackObject_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_Field", new Action( instance.TestMessagePackObjectArray_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArray", new Action( instance.TestMessagePackObjectArray_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArrayNull", new Action( instance.TestMessagePackObjectArray_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldNull", new Action( instance.TestMessagePackObjectArray_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32Field", new Action( instance.TestMinInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32FieldArray", new Action( instance.TestMinInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64Field", new Action( instance.TestMinInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64FieldArray", new Action( instance.TestMinInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArray", new Action( instance.TestMultiDimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArrayComprex", new Action( instance.TestMultiDimensionalArrayComprex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection", new Action( instance.TestNameValueCollection ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection_NullKey", new Action( instance.TestNameValueCollection_NullKey ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_Fail", new Action( instance.TestNoMembers_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableFail", new Action( instance.TestNoMembers_PackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableUnpackableSuccess", new Action( instance.TestNoMembers_PackableUnpackableSuccess ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_UnpackableFail", new Action( instance.TestNoMembers_UnpackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_Aware", new Action( instance.TestNonGenericCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_NotAware", new Action( instance.TestNonGenericCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_Aware", new Action( instance.TestNonGenericCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_NotAware", new Action( instance.TestNonGenericCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_Aware", new Action( instance.TestNonGenericCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_NotAware", new Action( instance.TestNonGenericCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_Aware", new Action( instance.TestNonGenericDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_NotAware", new Action( instance.TestNonGenericDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_Aware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_NotAware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_Aware", new Action( instance.TestNonGenericDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_NotAware", new Action( instance.TestNonGenericDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_Aware", new Action( instance.TestNonGenericEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_NotAware", new Action( instance.TestNonGenericEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_Aware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_NotAware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_Aware", new Action( instance.TestNonGenericEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_NotAware", new Action( instance.TestNonGenericEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_Aware", new Action( instance.TestNonGenericList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_NotAware", new Action( instance.TestNonGenericList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_Aware", new Action( instance.TestNonGenericList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_NotAware", new Action( instance.TestNonGenericList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_Aware", new Action( instance.TestNonGenericList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_NotAware", new Action( instance.TestNonGenericList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_DataContract_Failed", new Action( instance.TestNonPublicType_DataContract_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_MessagePackMember_Failed", new Action( instance.TestNonPublicType_MessagePackMember_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_Plain_Failed", new Action( instance.TestNonPublicType_Plain_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_DataContract", new Action( instance.TestNonPublicWritableMember_DataContract ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_MessagePackMember", new Action( instance.TestNonPublicWritableMember_MessagePackMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_PlainOldCliClass", new Action( instance.TestNonPublicWritableMember_PlainOldCliClass ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonZeroBoundMultidimensionalArray", new Action( instance.TestNonZeroBoundMultidimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullable", new Action( instance.TestNullable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullField", new Action( instance.TestNullField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArray", new Action( instance.TestNullFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArrayNull", new Action( instance.TestNullFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldNull", new Action( instance.TestNullFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayField", new Action( instance.TestObjectArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArray", new Action( instance.TestObjectArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArrayNull", new Action( instance.TestObjectArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldNull", new Action( instance.TestObjectArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectField", new Action( instance.TestObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArray", new Action( instance.TestObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArrayNull", new Action( instance.TestObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldNull", new Action( instance.TestObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Null_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphismAttributesInType", new Action( instance.TestPolymorphismAttributesInType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestReadOnlyAndConstructor", new Action( instance.TestReadOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStaticMembersDoNotCausePrepareError", new Action( instance.TestStaticMembersDoNotCausePrepareError ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestString", new Action( instance.TestString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringField", new Action( instance.TestStringField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArray", new Action( instance.TestStringFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArrayNull", new Action( instance.TestStringFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldNull", new Action( instance.TestStringFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeField", new Action( instance.TestStringKeyedCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArray", new Action( instance.TestStringKeyedCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArrayNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectField", new Action( instance.TestStringKeyedCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArray", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectField", new Action( instance.TestStringKeyedCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArray", new Action( instance.TestStringKeyedCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldNull", new Action( instance.TestStringKeyedCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanField", new Action( instance.TestTimeSpanField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanFieldArray", new Action( instance.TestTimeSpanFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteField", new Action( instance.TestTinyByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteFieldArray", new Action( instance.TestTinyByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32Field", new Action( instance.TestTinyInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32FieldArray", new Action( instance.TestTinyInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64Field", new Action( instance.TestTinyInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64FieldArray", new Action( instance.TestTinyInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16Field", new Action( instance.TestTinyUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16FieldArray", new Action( instance.TestTinyUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_Complex", new Action( instance.TestToFromMessagePackObject_Complex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_ComplexGenerated", new Action( instance.TestToFromMessagePackObject_ComplexGenerated ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueField", new Action( instance.TestTrueField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueFieldArray", new Action( instance.TestTrueFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierDoesNotLoadTypeItself", new Action( instance.TestTypeVerifierDoesNotLoadTypeItself ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo", new Action( instance.TestUnpackTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUri", new Action( instance.TestUri ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriField", new Action( instance.TestUriField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArray", new Action( instance.TestUriFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArrayNull", new Action( instance.TestUriFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldNull", new Action( instance.TestUriFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestValueType_Success", new Action( instance.TestValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinor", new Action( instance.TestVersionConstructorMajorMinor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArray", new Action( instance.TestVersionConstructorMajorMinorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArrayNull", new Action( instance.TestVersionConstructorMajorMinorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuild", new Action( instance.TestVersionConstructorMajorMinorBuild ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArray", new Action( instance.TestVersionConstructorMajorMinorBuildArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArrayNull", new Action( instance.TestVersionConstructorMajorMinorBuildArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildNull", new Action( instance.TestVersionConstructorMajorMinorBuildNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorNull", new Action( instance.TestVersionConstructorMajorMinorNull ) ) );
		}
	} 

	internal static class MapReflectionBasedEnumSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new MapReflectionBasedEnumSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MapReflectionBasedEnumSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByName", new Action( instance.TestEnumByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByName", new Action( instance.TestEnumByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByName", new Action( instance.TestEnumInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByName", new Action( instance.TestEnumInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByName", new Action( instance.TestEnumInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByName", new Action( instance.TestEnumInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByName", new Action( instance.TestEnumInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByName", new Action( instance.TestEnumInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_AllUpper", new Action( instance.TestEnumKeyTransformer_AllUpper ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Custom", new Action( instance.TestEnumKeyTransformer_Custom ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_Default_AsIs", new Action( instance.TestEnumKeyTransformer_Default_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumKeyTransformer_LowerCamel", new Action( instance.TestEnumKeyTransformer_LowerCamel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByName", new Action( instance.TestEnumSByte_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByName", new Action( instance.TestEnumSByte_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumSByte_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumSByte_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByName", new Action( instance.TestEnumUInt16_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByName", new Action( instance.TestEnumUInt16_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt16_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt16_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByName", new Action( instance.TestEnumUInt32_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByName", new Action( instance.TestEnumUInt32_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt32_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt32_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByName", new Action( instance.TestEnumUInt64_WithFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByName", new Action( instance.TestEnumUInt64_WithoutFlags_ByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumUInt64_WithoutFlags_ByUnderlyingValue", new Action( instance.TestEnumUInt64_WithoutFlags_ByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByName_TypeIsNone_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsByUnderlyingValue_TypeIsNone_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByName_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsByUnderlyingValue_MemberIsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByName ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsByUnderlyingValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsDefault", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsNone", new Action( instance.TestSerializationMethod_ContextIsDefault_TypeIsNone_MemberIsNone ) ) );
		}
	} 

	internal static class MapReflectionBasedReflectionMessagePackSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new MapReflectionBasedReflectionMessagePackSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MapReflectionBasedReflectionMessagePackSerializerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionKnownType_Success", new Action( instance.TestAbstractClassCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionNoAttribute_Success", new Action( instance.TestAbstractClassCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassCollectionRuntimeType_Success", new Action( instance.TestAbstractClassCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyKnownType_Success", new Action( instance.TestAbstractClassDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyNoAttribute_Fail", new Action( instance.TestAbstractClassDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassDictKeyRuntimeType_Success", new Action( instance.TestAbstractClassDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemKnownType_Success", new Action( instance.TestAbstractClassListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemNoAttribute_Fail", new Action( instance.TestAbstractClassListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassListItemRuntimeType_Success", new Action( instance.TestAbstractClassListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberKnownType_Success", new Action( instance.TestAbstractClassMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberNoAttribute_Fail", new Action( instance.TestAbstractClassMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractClassMemberRuntimeType_Success", new Action( instance.TestAbstractClassMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_Default_Success", new Action( instance.TestAbstractTypes_KnownCollections_Default_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistration_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistration_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success", new Action( instance.TestAbstractTypes_KnownCollections_ExplicitRegistrationForSpecific_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_KnownCollections_WithoutRegistration_Fail", new Action( instance.TestAbstractTypes_KnownCollections_WithoutRegistration_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAbstractTypes_NotACollection_Fail", new Action( instance.TestAbstractTypes_NotACollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeField", new Action( instance.TestAddOnlyCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArray", new Action( instance.TestAddOnlyCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldArrayNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_DateTimeFieldNull", new Action( instance.TestAddOnlyCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectField", new Action( instance.TestAddOnlyCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArray", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_MessagePackObjectFieldNull", new Action( instance.TestAddOnlyCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectField", new Action( instance.TestAddOnlyCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArray", new Action( instance.TestAddOnlyCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldArrayNull", new Action( instance.TestAddOnlyCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddOnlyCollection_ObjectFieldNull", new Action( instance.TestAddOnlyCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListField", new Action( instance.TestArrayListField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListFieldArray", new Action( instance.TestArrayListFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListFieldArrayNull", new Action( instance.TestArrayListFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArrayListFieldNull", new Action( instance.TestArrayListFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteField", new Action( instance.TestArraySegmentByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentByteFieldArray", new Action( instance.TestArraySegmentByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalField", new Action( instance.TestArraySegmentDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentDecimalFieldArray", new Action( instance.TestArraySegmentDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32Field", new Action( instance.TestArraySegmentInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArraySegmentInt32FieldArray", new Action( instance.TestArraySegmentInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoDefaultConstructor_Packable", new Action( instance.TestAsymmetric_PackOnly_NoDefaultConstructor_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableMultipleConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable", new Action( instance.TestAsymmetric_PackOnly_NoSettableNoConstructors_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnappendableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericCollection_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericDictionary_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericEnumerable_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable", new Action( instance.TestAsymmetric_PackOnly_UnconstructableNonGenericList_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable", new Action( instance.TestAsymmetric_PackOnly_UnsettableArrayMemberObject_Packable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownCollectionItem_Fail", new Action( instance.TestAttribute_DuplicatedKnownCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownDictionaryKey_Fail", new Action( instance.TestAttribute_DuplicatedKnownDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_DuplicatedKnownMember_Fail", new Action( instance.TestAttribute_DuplicatedKnownMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeCollectionItem_Fail", new Action( instance.TestAttribute_KnownAndRuntimeCollectionItem_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeDictionaryKey_Fail", new Action( instance.TestAttribute_KnownAndRuntimeDictionaryKey_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAttribute_KnownAndRuntimeMember_Fail", new Action( instance.TestAttribute_KnownAndRuntimeMember_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBinary_ClassicContext", new Action( instance.TestBinary_ClassicContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBinary_ContextWithPackerCompatilibyOptionsNone", new Action( instance.TestBinary_ContextWithPackerCompatilibyOptionsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayContent", new Action( instance.TestByteArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayField", new Action( instance.TestByteArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArray", new Action( instance.TestByteArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldArrayNull", new Action( instance.TestByteArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteArrayFieldNull", new Action( instance.TestByteArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteField", new Action( instance.TestByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByteFieldArray", new Action( instance.TestByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayContent", new Action( instance.TestCharArrayContent ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayField", new Action( instance.TestCharArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArray", new Action( instance.TestCharArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldArrayNull", new Action( instance.TestCharArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharArrayFieldNull", new Action( instance.TestCharArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharField", new Action( instance.TestCharField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCharFieldArray", new Action( instance.TestCharFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectField", new Action( instance.TestCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArray", new Action( instance.TestCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_MessagePackObjectFieldNull", new Action( instance.TestCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_Aware", new Action( instance.TestCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Packable_NotAware", new Action( instance.TestCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_Aware", new Action( instance.TestCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_PackableUnpackable_NotAware", new Action( instance.TestCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Success", new Action( instance.TestCollection_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_Aware", new Action( instance.TestCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollection_Unpackable_NotAware", new Action( instance.TestCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeField", new Action( instance.TestCollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArray", new Action( instance.TestCollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldArrayNull", new Action( instance.TestCollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionDateTimeFieldNull", new Action( instance.TestCollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectField", new Action( instance.TestCollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArray", new Action( instance.TestCollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldArrayNull", new Action( instance.TestCollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionObjectFieldNull", new Action( instance.TestCollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObject_WithoutShortcut", new Action( instance.TestComplexObject_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObject_WithShortcut", new Action( instance.TestComplexObject_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithDataContract_WithoutShortcut", new Action( instance.TestComplexObjectTypeWithDataContract_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithDataContract_WithShortcut", new Action( instance.TestComplexObjectTypeWithDataContract_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithNonSerialized_WithoutShortcut", new Action( instance.TestComplexObjectTypeWithNonSerialized_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexObjectTypeWithNonSerialized_WithShortcut", new Action( instance.TestComplexObjectTypeWithNonSerialized_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGenerated_WithoutShortcut", new Action( instance.TestComplexTypeGenerated_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGenerated_WithShortcut", new Action( instance.TestComplexTypeGenerated_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedArray_WithoutShortcut", new Action( instance.TestComplexTypeGeneratedArray_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedArray_WithShortcut", new Action( instance.TestComplexTypeGeneratedArray_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosure_WithoutShortcut", new Action( instance.TestComplexTypeGeneratedEnclosure_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosure_WithShortcut", new Action( instance.TestComplexTypeGeneratedEnclosure_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosureArray_WithoutShortcut", new Action( instance.TestComplexTypeGeneratedEnclosureArray_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeGeneratedEnclosureArray_WithShortcut", new Action( instance.TestComplexTypeGeneratedEnclosureArray_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithDataContractWithOrder_WithoutShortcut", new Action( instance.TestComplexTypeWithDataContractWithOrder_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithDataContractWithOrder_WithShortcut", new Action( instance.TestComplexTypeWithDataContractWithOrder_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithoutAnyAttribute_WithoutShortcut", new Action( instance.TestComplexTypeWithoutAnyAttribute_WithoutShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestComplexTypeWithoutAnyAttribute_WithShortcut", new Action( instance.TestComplexTypeWithoutAnyAttribute_WithShortcut ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstructorDeserializationWithParametersNotInLexicalOrder", new Action( instance.TestConstructorDeserializationWithParametersNotInLexicalOrder ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithAnotherNameConstrtor_DifferIsSetDefault", new Action( instance.TestConstrutorDeserializationWithAnotherNameConstrtor_DifferIsSetDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithAnotherTypeConstrtor_DifferIsSetDefault", new Action( instance.TestConstrutorDeserializationWithAnotherTypeConstrtor_DifferIsSetDefault ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithAttribute_Preferred", new Action( instance.TestConstrutorDeserializationWithAttribute_Preferred ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstrutorDeserializationWithMultipleAttributes_Fail", new Action( instance.TestConstrutorDeserializationWithMultipleAttributes_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeNamedProperties", new Action( instance.TestDataMemberAttributeNamedProperties ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBase", new Action( instance.TestDataMemberAttributeOrderWithOneBase ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBase_ProtoBufCompatible", new Action( instance.TestDataMemberAttributeOrderWithOneBase_ProtoBufCompatible ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBaseDeserialize", new Action( instance.TestDataMemberAttributeOrderWithOneBaseDeserialize ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithOneBaseDeserialize_ProtoBufCompatible", new Action( instance.TestDataMemberAttributeOrderWithOneBaseDeserialize_ProtoBufCompatible ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDataMemberAttributeOrderWithZeroBase_ProtoBufCompatible_Fail", new Action( instance.TestDataMemberAttributeOrderWithZeroBase_ProtoBufCompatible_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTime", new Action( instance.TestDateTime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeClassic", new Action( instance.TestDateTimeClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeField", new Action( instance.TestDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeFieldArray", new Action( instance.TestDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Local", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_NativeContext_Utc", new Action( instance.TestDateTimeMemberAttributes_NativeContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Local", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Local ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeMemberAttributes_UnixEpocContext_Utc", new Action( instance.TestDateTimeMemberAttributes_UnixEpocContext_Utc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeNullableChangeOnDemand", new Action( instance.TestDateTimeNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffset", new Action( instance.TestDateTimeOffset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetClassic", new Action( instance.TestDateTimeOffsetClassic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetField", new Action( instance.TestDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetFieldArray", new Action( instance.TestDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeOffsetNullableChangeOnDemand", new Action( instance.TestDateTimeOffsetNullableChangeOnDemand ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalField", new Action( instance.TestDecimalField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecimalFieldArray", new Action( instance.TestDecimalFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_Aware", new Action( instance.TestDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Packable_NotAware", new Action( instance.TestDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_Aware", new Action( instance.TestDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_PackableUnpackable_NotAware", new Action( instance.TestDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_Aware", new Action( instance.TestDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Unpackable_NotAware", new Action( instance.TestDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryField", new Action( instance.TestDictionaryEntryField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryEntryFieldArray", new Action( instance.TestDictionaryEntryFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryKeyTransformer_Custom", new Action( instance.TestDictionaryKeyTransformer_Custom ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryKeyTransformer_Default_AsIs", new Action( instance.TestDictionaryKeyTransformer_Default_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryKeyTransformer_LowerCamel", new Action( instance.TestDictionaryKeyTransformer_LowerCamel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectField", new Action( instance.TestDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArray", new Action( instance.TestDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldArrayNull", new Action( instance.TestDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryObjectObjectFieldNull", new Action( instance.TestDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeField", new Action( instance.TestDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArray", new Action( instance.TestDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionaryStringDateTimeFieldNull", new Action( instance.TestDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes", new Action( instance.TestEmptyBytes ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyBytes_Classic", new Action( instance.TestEmptyBytes_Classic ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyIntArray", new Action( instance.TestEmptyIntArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyKeyValuePairArray", new Action( instance.TestEmptyKeyValuePairArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyMap", new Action( instance.TestEmptyMap ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEmptyString", new Action( instance.TestEmptyString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnum", new Action( instance.TestEnum ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_Aware", new Action( instance.TestEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Packable_NotAware", new Action( instance.TestEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_Aware", new Action( instance.TestEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_PackableUnpackable_NotAware", new Action( instance.TestEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_Aware", new Action( instance.TestEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEnumerable_Unpackable_NotAware", new Action( instance.TestEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExplicitlyImplementedPackableUnpackable", new Action( instance.TestExplicitlyImplementedPackableUnpackable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ClassicContext", new Action( instance.TestExt_ClassicContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_ContextWithPackerCompatilibyOptionsNone", new Action( instance.TestExt_ContextWithPackerCompatilibyOptionsNone ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExt_DefaultContext", new Action( instance.TestExt_DefaultContext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseField", new Action( instance.TestFalseField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFalseFieldArray", new Action( instance.TestFalseFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEField", new Action( instance.TestFILETIMEField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFILETIMEFieldArray", new Action( instance.TestFILETIMEFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructor", new Action( instance.TestFullVersionConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArray", new Action( instance.TestFullVersionConstructorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorArrayNull", new Action( instance.TestFullVersionConstructorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFullVersionConstructorNull", new Action( instance.TestFullVersionConstructorNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetOnlyAndConstructor", new Action( instance.TestGetOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGlobalNamespace", new Action( instance.TestGlobalNamespace ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidField", new Action( instance.TestGuidField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGuidFieldArray", new Action( instance.TestGuidFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_Success", new Action( instance.TestHasGetOnlyAppendableCollectionPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasGetOnlyPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithBothConstructor_Success", new Action( instance.TestHasGetOnlyPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithDefaultConstructor_Fail", new Action( instance.TestHasGetOnlyPropertyWithDefaultConstructor_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasGetOnlyPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasGetOnlyPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasGetOnlyPropertyWithRecordConstructor_Success", new Action( instance.TestHasGetOnlyPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectField", new Action( instance.TestHashSet_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArray", new Action( instance.TestHashSet_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldArrayNull", new Action( instance.TestHashSet_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSet_MessagePackObjectFieldNull", new Action( instance.TestHashSet_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeField", new Action( instance.TestHashSetDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArray", new Action( instance.TestHashSetDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldArrayNull", new Action( instance.TestHashSetDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetDateTimeFieldNull", new Action( instance.TestHashSetDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectField", new Action( instance.TestHashSetObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArray", new Action( instance.TestHashSetObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldArrayNull", new Action( instance.TestHashSetObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashSetObjectFieldNull", new Action( instance.TestHashSetObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableField", new Action( instance.TestHashtableField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableFieldArray", new Action( instance.TestHashtableFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableFieldArrayNull", new Action( instance.TestHashtableFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHashtableFieldNull", new Action( instance.TestHashtableFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_Success", new Action( instance.TestHasInitOnlyAppendableCollectionFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasInitOnlyFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithBothConstructor_Success", new Action( instance.TestHasInitOnlyFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithDefaultConstructor_Fail", new Action( instance.TestHasInitOnlyFieldWithDefaultConstructor_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasInitOnlyFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasInitOnlyFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasInitOnlyFieldWithRecordConstructor_Success", new Action( instance.TestHasInitOnlyFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_Success", new Action( instance.TestHasPrivateSetterAppendableCollectionPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithBothConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPrivateSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPrivateSetterPropertyWithRecordConstructor_Success", new Action( instance.TestHasPrivateSetterPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_Success", new Action( instance.TestHasPublicSetterAppendableCollectionPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithAnnotatedConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithBothConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithDefaultConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasPublicSetterPropertyWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasPublicSetterPropertyWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasPublicSetterPropertyWithRecordConstructor_Success", new Action( instance.TestHasPublicSetterPropertyWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithBothConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_Success", new Action( instance.TestHasReadWriteAppendableCollectionFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithAnnotatedConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithAnnotatedConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithAnnotatedConstructor_Success", new Action( instance.TestHasReadWriteFieldWithAnnotatedConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithBothConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithBothConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithBothConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithBothConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithBothConstructor_Success", new Action( instance.TestHasReadWriteFieldWithBothConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithDefaultConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithDefaultConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithDefaultConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithDefaultConstructor_Success", new Action( instance.TestHasReadWriteFieldWithDefaultConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success", new Action( instance.TestHasReadWriteFieldWithRecordConstructor_DeseriaizeWithExtraMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithRecordConstructor_DeserializeWithMissingMember_Success", new Action( instance.TestHasReadWriteFieldWithRecordConstructor_DeserializeWithMissingMember_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHasReadWriteFieldWithRecordConstructor_Success", new Action( instance.TestHasReadWriteFieldWithRecordConstructor_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectField", new Action( instance.TestICollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArray", new Action( instance.TestICollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldArrayNull", new Action( instance.TestICollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollection_MessagePackObjectFieldNull", new Action( instance.TestICollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeField", new Action( instance.TestICollectionDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArray", new Action( instance.TestICollectionDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldArrayNull", new Action( instance.TestICollectionDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionDateTimeFieldNull", new Action( instance.TestICollectionDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectField", new Action( instance.TestICollectionObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArray", new Action( instance.TestICollectionObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldArrayNull", new Action( instance.TestICollectionObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionObjectFieldNull", new Action( instance.TestICollectionObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectField", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArray", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionary_MessagePackObject_MessagePackObjectFieldNull", new Action( instance.TestIDictionary_MessagePackObject_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectField", new Action( instance.TestIDictionaryObjectObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArray", new Action( instance.TestIDictionaryObjectObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldArrayNull", new Action( instance.TestIDictionaryObjectObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryObjectObjectFieldNull", new Action( instance.TestIDictionaryObjectObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeField", new Action( instance.TestIDictionaryStringDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArray", new Action( instance.TestIDictionaryStringDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldArrayNull", new Action( instance.TestIDictionaryStringDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryStringDateTimeFieldNull", new Action( instance.TestIDictionaryStringDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryValueType_Success", new Action( instance.TestIDictionaryValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_ExcludedOnly", new Action( instance.TestIgnore_ExcludedOnly ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_ExclusionAndInclusionMixed", new Action( instance.TestIgnore_ExclusionAndInclusionMixed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_ExclusionAndInclusionSimulatously", new Action( instance.TestIgnore_ExclusionAndInclusionSimulatously ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIgnore_Normal", new Action( instance.TestIgnore_Normal ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectField", new Action( instance.TestIList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArray", new Action( instance.TestIList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldArrayNull", new Action( instance.TestIList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIList_MessagePackObjectFieldNull", new Action( instance.TestIList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeField", new Action( instance.TestIListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArray", new Action( instance.TestIListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldArrayNull", new Action( instance.TestIListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListDateTimeFieldNull", new Action( instance.TestIListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectField", new Action( instance.TestIListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArray", new Action( instance.TestIListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldArrayNull", new Action( instance.TestIListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListObjectFieldNull", new Action( instance.TestIListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIListValueType_Success", new Action( instance.TestIListValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_Field", new Action( instance.TestImage_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArray", new Action( instance.TestImage_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldArrayNull", new Action( instance.TestImage_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage_FieldNull", new Action( instance.TestImage_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail", new Action( instance.TestImplementsGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsNonGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail", new Action( instance.TestImplementsNonGenericIEnumerableWithNoAdd_ProhibitEnumerableNonCollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImplementsNonGenericIEnumerableWithNoAdd_Success", new Action( instance.TestImplementsNonGenericIEnumerableWithNoAdd_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32", new Action( instance.TestInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64", new Action( instance.TestInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionKnownType_Success", new Action( instance.TestInterfaceCollectionKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionNoAttribute_Success", new Action( instance.TestInterfaceCollectionNoAttribute_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceCollectionRuntimeType_Success", new Action( instance.TestInterfaceCollectionRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyKnownType_Success", new Action( instance.TestInterfaceDictKeyKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyNoAttribute_Fail", new Action( instance.TestInterfaceDictKeyNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceDictKeyRuntimeType_Success", new Action( instance.TestInterfaceDictKeyRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemKnownType_Success", new Action( instance.TestInterfaceListItemKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemNoAttribute_Fail", new Action( instance.TestInterfaceListItemNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceListItemRuntimeType_Success", new Action( instance.TestInterfaceListItemRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberKnownType_Success", new Action( instance.TestInterfaceMemberKnownType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberNoAttribute_Fail", new Action( instance.TestInterfaceMemberNoAttribute_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInterfaceMemberRuntimeType_Success", new Action( instance.TestInterfaceMemberRuntimeType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_Plain", new Action( instance.TestIssue25_Plain ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue25_SelfComposite", new Action( instance.TestIssue25_SelfComposite ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetField", new Action( instance.TestKeyValuePairStringDateTimeOffsetField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeyValuePairStringDateTimeOffsetFieldArray", new Action( instance.TestKeyValuePairStringDateTimeOffsetFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Field_Known", new Action( instance.TestKnownType_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsKnown_Property_Known", new Action( instance.TestKnownType_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Field_Known", new Action( instance.TestKnownType_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsNothing_Property_Known", new Action( instance.TestKnownType_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeCollection_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsKnown_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsKnown_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Field_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Field_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsNothing_Property_Known", new Action( instance.TestKnownTypeDictionary_AttributeIsNothing_Property_Known ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestKnownTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectField", new Action( instance.TestList_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArray", new Action( instance.TestList_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldArrayNull", new Action( instance.TestList_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_MessagePackObjectFieldNull", new Action( instance.TestList_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_Aware", new Action( instance.TestList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Packable_NotAware", new Action( instance.TestList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_Aware", new Action( instance.TestList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_PackableUnpackable_NotAware", new Action( instance.TestList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_Aware", new Action( instance.TestList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestList_Unpackable_NotAware", new Action( instance.TestList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeField", new Action( instance.TestListDateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArray", new Action( instance.TestListDateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldArrayNull", new Action( instance.TestListDateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListDateTimeFieldNull", new Action( instance.TestListDateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectField", new Action( instance.TestListObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArray", new Action( instance.TestListObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldArrayNull", new Action( instance.TestListObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestListObjectFieldNull", new Action( instance.TestListObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestManyMembers", new Action( instance.TestManyMembers ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteField", new Action( instance.TestMaxByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxByteFieldArray", new Action( instance.TestMaxByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32Field", new Action( instance.TestMaxInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt32FieldArray", new Action( instance.TestMaxInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64Field", new Action( instance.TestMaxInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxInt64FieldArray", new Action( instance.TestMaxInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16Field", new Action( instance.TestMaxUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMaxUInt16FieldArray", new Action( instance.TestMaxUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_Field", new Action( instance.TestMessagePackObject_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArray", new Action( instance.TestMessagePackObject_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldArrayNull", new Action( instance.TestMessagePackObject_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_FieldNull", new Action( instance.TestMessagePackObject_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_Field", new Action( instance.TestMessagePackObjectArray_Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArray", new Action( instance.TestMessagePackObjectArray_FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldArrayNull", new Action( instance.TestMessagePackObjectArray_FieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObjectArray_FieldNull", new Action( instance.TestMessagePackObjectArray_FieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32Field", new Action( instance.TestMinInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt32FieldArray", new Action( instance.TestMinInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64Field", new Action( instance.TestMinInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMinInt64FieldArray", new Action( instance.TestMinInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArray", new Action( instance.TestMultiDimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultiDimensionalArrayComprex", new Action( instance.TestMultiDimensionalArrayComprex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection", new Action( instance.TestNameValueCollection ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNameValueCollection_NullKey", new Action( instance.TestNameValueCollection_NullKey ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_Fail", new Action( instance.TestNoMembers_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableFail", new Action( instance.TestNoMembers_PackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_PackableUnpackableSuccess", new Action( instance.TestNoMembers_PackableUnpackableSuccess ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNoMembers_UnpackableFail", new Action( instance.TestNoMembers_UnpackableFail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_Aware", new Action( instance.TestNonGenericCollection_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Packable_NotAware", new Action( instance.TestNonGenericCollection_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_Aware", new Action( instance.TestNonGenericCollection_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_PackableUnpackable_NotAware", new Action( instance.TestNonGenericCollection_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_Aware", new Action( instance.TestNonGenericCollection_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericCollection_Unpackable_NotAware", new Action( instance.TestNonGenericCollection_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_Aware", new Action( instance.TestNonGenericDictionary_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Packable_NotAware", new Action( instance.TestNonGenericDictionary_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_Aware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_PackableUnpackable_NotAware", new Action( instance.TestNonGenericDictionary_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_Aware", new Action( instance.TestNonGenericDictionary_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericDictionary_Unpackable_NotAware", new Action( instance.TestNonGenericDictionary_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_Aware", new Action( instance.TestNonGenericEnumerable_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Packable_NotAware", new Action( instance.TestNonGenericEnumerable_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_Aware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_PackableUnpackable_NotAware", new Action( instance.TestNonGenericEnumerable_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_Aware", new Action( instance.TestNonGenericEnumerable_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericEnumerable_Unpackable_NotAware", new Action( instance.TestNonGenericEnumerable_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_Aware", new Action( instance.TestNonGenericList_Packable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Packable_NotAware", new Action( instance.TestNonGenericList_Packable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_Aware", new Action( instance.TestNonGenericList_PackableUnpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_PackableUnpackable_NotAware", new Action( instance.TestNonGenericList_PackableUnpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_Aware", new Action( instance.TestNonGenericList_Unpackable_Aware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonGenericList_Unpackable_NotAware", new Action( instance.TestNonGenericList_Unpackable_NotAware ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_DataContract_Failed", new Action( instance.TestNonPublicType_DataContract_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_MessagePackMember_Failed", new Action( instance.TestNonPublicType_MessagePackMember_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicType_Plain_Failed", new Action( instance.TestNonPublicType_Plain_Failed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_DataContract", new Action( instance.TestNonPublicWritableMember_DataContract ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_MessagePackMember", new Action( instance.TestNonPublicWritableMember_MessagePackMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonPublicWritableMember_PlainOldCliClass", new Action( instance.TestNonPublicWritableMember_PlainOldCliClass ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNonZeroBoundMultidimensionalArray", new Action( instance.TestNonZeroBoundMultidimensionalArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullable", new Action( instance.TestNullable ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullField", new Action( instance.TestNullField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArray", new Action( instance.TestNullFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldArrayNull", new Action( instance.TestNullFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNullFieldNull", new Action( instance.TestNullFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayField", new Action( instance.TestObjectArrayField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArray", new Action( instance.TestObjectArrayFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldArrayNull", new Action( instance.TestObjectArrayFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectArrayFieldNull", new Action( instance.TestObjectArrayFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectField", new Action( instance.TestObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArray", new Action( instance.TestObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldArrayNull", new Action( instance.TestObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestObjectFieldNull", new Action( instance.TestObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOmitNullEntryInDictionary", new Action( instance.TestOmitNullEntryInDictionary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOmitNullEntryInDictionary_BackwordCompatibility", new Action( instance.TestOmitNullEntryInDictionary_BackwordCompatibility ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorBoolean_Success", new Action( instance.TestOptionalConstructorBoolean_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorByte_Success", new Action( instance.TestOptionalConstructorByte_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorChar_Success", new Action( instance.TestOptionalConstructorChar_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorDecimal_Success", new Action( instance.TestOptionalConstructorDecimal_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorDouble_Success", new Action( instance.TestOptionalConstructorDouble_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorInt16_Success", new Action( instance.TestOptionalConstructorInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorInt32_Success", new Action( instance.TestOptionalConstructorInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorInt64_Success", new Action( instance.TestOptionalConstructorInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorSByte_Success", new Action( instance.TestOptionalConstructorSByte_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorSingle_Success", new Action( instance.TestOptionalConstructorSingle_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorString_Success", new Action( instance.TestOptionalConstructorString_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorUInt16_Success", new Action( instance.TestOptionalConstructorUInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorUInt32_Success", new Action( instance.TestOptionalConstructorUInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOptionalConstructorUInt64_Success", new Action( instance.TestOptionalConstructorUInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackable_PackToMessageUsed", new Action( instance.TestPackable_PackToMessageUsed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackableUnpackable_PackToMessageAndUnpackFromMessageUsed", new Action( instance.TestPackableUnpackable_PackToMessageAndUnpackFromMessageUsed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Null_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphicMemberTypeMixed_Success", new Action( instance.TestPolymorphicMemberTypeMixed_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPolymorphismAttributesInType", new Action( instance.TestPolymorphismAttributesInType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestReadOnlyAndConstructor", new Action( instance.TestReadOnlyAndConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeType_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeType_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeType_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeCollection_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeCollection_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Field_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail", new Action( instance.TestRuntimeTypeDictionary_AttributeIsKnown_Property_Known_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsNothing_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Field_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime", new Action( instance.TestRuntimeTypeDictionary_AttributeIsRuntime_Property_Runtime ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseExtraParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoMethods_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseNoParametersMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail", new Action( instance.TestSpecifiedTypeVerifierIsNotFound_BecauseVoidReturnMethod_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStaticMembersDoNotCausePrepareError", new Action( instance.TestStaticMembersDoNotCausePrepareError ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestString", new Action( instance.TestString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringField", new Action( instance.TestStringField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArray", new Action( instance.TestStringFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldArrayNull", new Action( instance.TestStringFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringFieldNull", new Action( instance.TestStringFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeField", new Action( instance.TestStringKeyedCollection_DateTimeField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArray", new Action( instance.TestStringKeyedCollection_DateTimeFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldArrayNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_DateTimeFieldNull", new Action( instance.TestStringKeyedCollection_DateTimeFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectField", new Action( instance.TestStringKeyedCollection_MessagePackObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArray", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_MessagePackObjectFieldNull", new Action( instance.TestStringKeyedCollection_MessagePackObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectField", new Action( instance.TestStringKeyedCollection_ObjectField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArray", new Action( instance.TestStringKeyedCollection_ObjectFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldArrayNull", new Action( instance.TestStringKeyedCollection_ObjectFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringKeyedCollection_ObjectFieldNull", new Action( instance.TestStringKeyedCollection_ObjectFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanField", new Action( instance.TestTimeSpanField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTimeSpanFieldArray", new Action( instance.TestTimeSpanFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteField", new Action( instance.TestTinyByteField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyByteFieldArray", new Action( instance.TestTinyByteFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32Field", new Action( instance.TestTinyInt32Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt32FieldArray", new Action( instance.TestTinyInt32FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64Field", new Action( instance.TestTinyInt64Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyInt64FieldArray", new Action( instance.TestTinyInt64FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16Field", new Action( instance.TestTinyUInt16Field ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTinyUInt16FieldArray", new Action( instance.TestTinyUInt16FieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_Complex", new Action( instance.TestToFromMessagePackObject_Complex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToFromMessagePackObject_ComplexGenerated", new Action( instance.TestToFromMessagePackObject_ComplexGenerated ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueField", new Action( instance.TestTrueField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTrueFieldArray", new Action( instance.TestTrueFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierDoesNotLoadTypeItself", new Action( instance.TestTypeVerifierDoesNotLoadTypeItself ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_NonPublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_NonPublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicInstanceMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK", new Action( instance.TestTypeVerifierSelection_PublicVerifierType_PublicStaticMethod_OK ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeWithDuplicatedMessagePackMemberAttributeMember", new Action( instance.TestTypeWithDuplicatedMessagePackMemberAttributeMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeWithInvalidMessagePackMemberAttributeMember", new Action( instance.TestTypeWithInvalidMessagePackMemberAttributeMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTypeWithMissingMessagePackMemberAttributeMember", new Action( instance.TestTypeWithMissingMessagePackMemberAttributeMember ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackable_UnpackFromMessageUsed", new Action( instance.TestUnpackable_UnpackFromMessageUsed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo", new Action( instance.TestUnpackTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUri", new Action( instance.TestUri ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriField", new Action( instance.TestUriField ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArray", new Action( instance.TestUriFieldArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldArrayNull", new Action( instance.TestUriFieldArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUriFieldNull", new Action( instance.TestUriFieldNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestValueType_Success", new Action( instance.TestValueType_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinor", new Action( instance.TestVersionConstructorMajorMinor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArray", new Action( instance.TestVersionConstructorMajorMinorArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorArrayNull", new Action( instance.TestVersionConstructorMajorMinorArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuild", new Action( instance.TestVersionConstructorMajorMinorBuild ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArray", new Action( instance.TestVersionConstructorMajorMinorBuildArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildArrayNull", new Action( instance.TestVersionConstructorMajorMinorBuildArrayNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorBuildNull", new Action( instance.TestVersionConstructorMajorMinorBuildNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestVersionConstructorMajorMinorNull", new Action( instance.TestVersionConstructorMajorMinorNull ) ) );
		}
	} 

	internal static class MessagePackConvertTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackConvertTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackConvertTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecodeStringStrict_Empty_Empty", new Action( instance.TestDecodeStringStrict_Empty_Empty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecodeStringStrict_Invalid", new Action( instance.TestDecodeStringStrict_Invalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecodeStringStrict_Normal_Success", new Action( instance.TestDecodeStringStrict_Normal_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecodeStringStrict_Null", new Action( instance.TestDecodeStringStrict_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDecodeStringStrict_WithBom_Success", new Action( instance.TestDecodeStringStrict_WithBom_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEncodeString_Empty_EncodedAsEmpty", new Action( instance.TestEncodeString_Empty_EncodedAsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEncodeString_Normal_EncodedAsUtf8NonBom", new Action( instance.TestEncodeString_Normal_EncodedAsUtf8NonBom ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEncodeString_Null", new Action( instance.TestEncodeString_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTime_MaxValue_AsUnixEpoc", new Action( instance.TestFromDateTime_MaxValue_AsUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTime_MinValue_AsUnixEpoc", new Action( instance.TestFromDateTime_MinValue_AsUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTime_Now_AsUtcUnixEpoc", new Action( instance.TestFromDateTime_Now_AsUtcUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTime_UtcEpoc_Zero", new Action( instance.TestFromDateTime_UtcEpoc_Zero ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTime_UtcNow_AsUnixEpoc", new Action( instance.TestFromDateTime_UtcNow_AsUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTimeOffset_MaxValue_AsUnixEpoc", new Action( instance.TestFromDateTimeOffset_MaxValue_AsUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTimeOffset_MinValue_AsUnixEpoc", new Action( instance.TestFromDateTimeOffset_MinValue_AsUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTimeOffset_Now_AsUtcUnixEpoc", new Action( instance.TestFromDateTimeOffset_Now_AsUtcUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTimeOffset_UtcEpoc_Zero", new Action( instance.TestFromDateTimeOffset_UtcEpoc_Zero ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromDateTimeOffset_UtcNow_AsUnixEpoc", new Action( instance.TestFromDateTimeOffset_UtcNow_AsUnixEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue43", new Action( instance.TestIssue43 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_Maximum_IsUtcEpoc", new Action( instance.TestToDateTime_Maximum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_MaximumPlusOne_IsUtcEpoc", new Action( instance.TestToDateTime_MaximumPlusOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_Minimum_IsUtcEpoc", new Action( instance.TestToDateTime_Minimum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_MinimumMinusOne_IsUtcEpoc", new Action( instance.TestToDateTime_MinimumMinusOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_MinusOne_IsUtcEpoc", new Action( instance.TestToDateTime_MinusOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_One_IsUtcEpoc", new Action( instance.TestToDateTime_One_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTime_Zero_IsUtcEpoc", new Action( instance.TestToDateTime_Zero_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_Maximum_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_Maximum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_MaximumPlusOne_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_MaximumPlusOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_Minimum_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_Minimum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_MinimumMinusOne_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_MinimumMinusOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_MinuOne_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_MinuOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_One_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_One_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffset_Zero_IsUtcEpoc", new Action( instance.TestToDateTimeOffset_Zero_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffsetRoundTrip_Maximum_IsUtcEpoc", new Action( instance.TestToDateTimeOffsetRoundTrip_Maximum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffsetRoundTrip_Minimum_IsUtcEpoc", new Action( instance.TestToDateTimeOffsetRoundTrip_Minimum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffsetRoundTrip_MinuOne_IsUtcEpoc", new Action( instance.TestToDateTimeOffsetRoundTrip_MinuOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffsetRoundTrip_One_IsUtcEpoc", new Action( instance.TestToDateTimeOffsetRoundTrip_One_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeOffsetRoundTrip_Zero_IsUtcEpoc", new Action( instance.TestToDateTimeOffsetRoundTrip_Zero_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeRoundTrip_Maximum_IsUtcEpoc", new Action( instance.TestToDateTimeRoundTrip_Maximum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeRoundTrip_Minimum_IsUtcEpoc", new Action( instance.TestToDateTimeRoundTrip_Minimum_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeRoundTrip_MinusOne_IsUtcEpoc", new Action( instance.TestToDateTimeRoundTrip_MinusOne_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeRoundTrip_One_IsUtcEpoc", new Action( instance.TestToDateTimeRoundTrip_One_IsUtcEpoc ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToDateTimeRoundTrip_Zero_IsUtcEpoc", new Action( instance.TestToDateTimeRoundTrip_Zero_IsUtcEpoc ) ) );
		}
	} 

	internal static class MessagePackExtendedTypeObjectTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackExtendedTypeObjectTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackExtendedTypeObjectTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConstructor", new Action( instance.TestConstructor ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquality_SameBody", new Action( instance.TestEquality_SameBody ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquality_Self", new Action( instance.TestEquality_Self ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquality_ValueEqual", new Action( instance.TestEquality_ValueEqual ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode", new Action( instance.TestGetHashCode ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGethashCode_Null", new Action( instance.TestGethashCode_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsValid", new Action( instance.TestIsValid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestProperties", new Action( instance.TestProperties ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString", new Action( instance.TestToString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_Null", new Action( instance.TestToString_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToStringJson", new Action( instance.TestToStringJson ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToStringJson_Null", new Action( instance.TestToStringJson_Null ) ) );
		}
	} 

	internal static class MessagePackObjectDictionaryTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectDictionaryTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectDictionaryTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "Clear_Iterators", new Action( instance.Clear_Iterators ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "Dictionary_MoveNext", new Action( instance.Dictionary_MoveNext ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "Empty_KeysValues_CopyTo", new Action( instance.Empty_KeysValues_CopyTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "Enumerator_Current", new Action( instance.Enumerator_Current ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "FailFastTest1", new Action( instance.FailFastTest1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "FailFastTest2", new Action( instance.FailFastTest2 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "FailFastTest3", new Action( instance.FailFastTest3 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ForEachTest", new Action( instance.ForEachTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionCopyTo", new Action( instance.ICollectionCopyTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionCopyTo_ex3", new Action( instance.ICollectionCopyTo_ex3 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionCopyTo_ex4", new Action( instance.ICollectionCopyTo_ex4 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionCopyToDictionaryEntryArray", new Action( instance.ICollectionCopyToDictionaryEntryArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionCopyToKeyValuePairArray", new Action( instance.ICollectionCopyToKeyValuePairArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionCopyToObjectArray", new Action( instance.ICollectionCopyToObjectArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionOfKeyValuePairContains", new Action( instance.ICollectionOfKeyValuePairContains ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ICollectionOfKeyValuePairRemove", new Action( instance.ICollectionOfKeyValuePairRemove ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IDictionary_Add_Null", new Action( instance.IDictionary_Add_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IDictionary_Contains", new Action( instance.IDictionary_Contains ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IDictionary_IndexerGetNonExistingTest", new Action( instance.IDictionary_IndexerGetNonExistingTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IDictionary_Remove1", new Action( instance.IDictionary_Remove1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IDictionaryEnumeratorTest", new Action( instance.IDictionaryEnumeratorTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IEnumeratorGenericTest", new Action( instance.IEnumeratorGenericTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "IEnumeratorTest", new Action( instance.IEnumeratorTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "KeyEnumerator_Current", new Action( instance.KeyEnumerator_Current ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "PlainEnumeratorReturnTest", new Action( instance.PlainEnumeratorReturnTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ResetKeysEnumerator", new Action( instance.ResetKeysEnumerator ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ResetShimEnumerator", new Action( instance.ResetShimEnumerator ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ResetValuesEnumerator", new Action( instance.ResetValuesEnumerator ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "SliceCollectionsEnumeratorTest", new Action( instance.SliceCollectionsEnumeratorTest ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddForEachRemoveClearContains", new Action( instance.TestAddForEachRemoveClearContains ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAddNull", new Action( instance.TestAddNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionTContainsNull", new Action( instance.TestCollectionTContainsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCollectionTRemoveNull", new Action( instance.TestCollectionTRemoveNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestContainsKeyNull", new Action( instance.TestContainsKeyNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCopyTo", new Action( instance.TestCopyTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetNull", new Action( instance.TestGetNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestICollectionTAddNull", new Action( instance.TestICollectionTAddNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryAddForEachRemoveClearContains", new Action( instance.TestIDictionaryAddForEachRemoveClearContains ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryAddMessagePackObjectNull", new Action( instance.TestIDictionaryAddMessagePackObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryAddObjectNull", new Action( instance.TestIDictionaryAddObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryContainsNull", new Action( instance.TestIDictionaryContainsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryGetMessagePackObjectNull", new Action( instance.TestIDictionaryGetMessagePackObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryGetObjectNull", new Action( instance.TestIDictionaryGetObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryIndexer", new Action( instance.TestIDictionaryIndexer ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryRemoveMessagePackObjectNull", new Action( instance.TestIDictionaryRemoveMessagePackObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionaryRemoveObjectNull", new Action( instance.TestIDictionaryRemoveObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionarySetMessagePackObjectNull", new Action( instance.TestIDictionarySetMessagePackObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIDictionarySetObjectNull", new Action( instance.TestIDictionarySetObjectNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIndexer", new Action( instance.TestIndexer ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeys", new Action( instance.TestKeys ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestKeysCopyTo", new Action( instance.TestKeysCopyTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestRemoveNull", new Action( instance.TestRemoveNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSetNull", new Action( instance.TestSetNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestTryGetValueNull", new Action( instance.TestTryGetValueNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestValues", new Action( instance.TestValues ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestValuesCopyTo", new Action( instance.TestValuesCopyTo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ValueEnumerator_Current", new Action( instance.ValueEnumerator_Current ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ValuesCopyToObjectArray", new Action( instance.ValuesCopyToObjectArray ) ) );
		}
	} 

	internal static class MessagePackObjectTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsByte", new Action( instance.TestAsByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsByteOverflow", new Action( instance.TestAsByteOverflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsByteUnderflow", new Action( instance.TestAsByteUnderflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsDouble", new Action( instance.TestAsDouble ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt16", new Action( instance.TestAsInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt16Overflow", new Action( instance.TestAsInt16Overflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt16Underflow", new Action( instance.TestAsInt16Underflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt32", new Action( instance.TestAsInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt32Overflow", new Action( instance.TestAsInt32Overflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt32Underflow", new Action( instance.TestAsInt32Underflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt64", new Action( instance.TestAsInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsInt64Overflow", new Action( instance.TestAsInt64Overflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsSByte", new Action( instance.TestAsSByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsSByteOverflow", new Action( instance.TestAsSByteOverflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsSByteUnderflow", new Action( instance.TestAsSByteUnderflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsSingle", new Action( instance.TestAsSingle ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt16", new Action( instance.TestAsUInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt16Overflow", new Action( instance.TestAsUInt16Overflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt16Underflow", new Action( instance.TestAsUInt16Underflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt32", new Action( instance.TestAsUInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt32Overflow", new Action( instance.TestAsUInt32Overflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt32Underflow", new Action( instance.TestAsUInt32Underflow ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt64", new Action( instance.TestAsUInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsUInt64Underflow", new Action( instance.TestAsUInt64Underflow ) ) );
		}
	} 

	internal static class MessagePackObjectTest_EqualsInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_Equals();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_Equals )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquality_ValueEqual", new Action( instance.TestEquality_ValueEqual ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Array_EqualArray_True", new Action( instance.TestEquals_Array_EqualArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Array_Null_False", new Action( instance.TestEquals_Array_Null_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Array_Raw_False", new Action( instance.TestEquals_Array_Raw_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Array_SameLengthButDifferArray_False", new Action( instance.TestEquals_Array_SameLengthButDifferArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Array_SubArray_False", new Action( instance.TestEquals_Array_SubArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Byte0x0_True", new Action( instance.TestEquals_Byte0x0_Byte0x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Byte0x0ff_False", new Action( instance.TestEquals_Byte0x0_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Byte0x1_False", new Action( instance.TestEquals_Byte0x0_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int160x0_True", new Action( instance.TestEquals_Byte0x0_Int160x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int160x0ff_False", new Action( instance.TestEquals_Byte0x0_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int160x1_False", new Action( instance.TestEquals_Byte0x0_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int160x80_False", new Action( instance.TestEquals_Byte0x0_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int160x8000_False", new Action( instance.TestEquals_Byte0x0_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int160xf_False", new Action( instance.TestEquals_Byte0x0_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320x0_True", new Action( instance.TestEquals_Byte0x0_Int320x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320x0ff_False", new Action( instance.TestEquals_Byte0x0_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320x1_False", new Action( instance.TestEquals_Byte0x0_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320x80_False", new Action( instance.TestEquals_Byte0x0_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320x8000_False", new Action( instance.TestEquals_Byte0x0_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320x80000000_False", new Action( instance.TestEquals_Byte0x0_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int320xf_False", new Action( instance.TestEquals_Byte0x0_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x0_True", new Action( instance.TestEquals_Byte0x0_Int640x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x0ff_False", new Action( instance.TestEquals_Byte0x0_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x1_False", new Action( instance.TestEquals_Byte0x0_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x80_False", new Action( instance.TestEquals_Byte0x0_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x8000_False", new Action( instance.TestEquals_Byte0x0_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x80000000_False", new Action( instance.TestEquals_Byte0x0_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640x8000000000000000_False", new Action( instance.TestEquals_Byte0x0_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_Int640xf_False", new Action( instance.TestEquals_Byte0x0_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_SByte0x0_True", new Action( instance.TestEquals_Byte0x0_SByte0x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_SByte0x1_False", new Action( instance.TestEquals_Byte0x0_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_SByte0x80_False", new Action( instance.TestEquals_Byte0x0_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_SByte0xf_False", new Action( instance.TestEquals_Byte0x0_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt160x0_True", new Action( instance.TestEquals_Byte0x0_UInt160x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt160x0ff_False", new Action( instance.TestEquals_Byte0x0_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt160x1_False", new Action( instance.TestEquals_Byte0x0_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt320x0_True", new Action( instance.TestEquals_Byte0x0_UInt320x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt320x0ff_False", new Action( instance.TestEquals_Byte0x0_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt320x1_False", new Action( instance.TestEquals_Byte0x0_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt640x0_True", new Action( instance.TestEquals_Byte0x0_UInt640x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt640x0ff_False", new Action( instance.TestEquals_Byte0x0_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_Byte0x0_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0_UInt640x1_False", new Action( instance.TestEquals_Byte0x0_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Byte0x0_False", new Action( instance.TestEquals_Byte0x0ff_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Byte0x0ff_True", new Action( instance.TestEquals_Byte0x0ff_Byte0x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Byte0x1_False", new Action( instance.TestEquals_Byte0x0ff_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int160x0_False", new Action( instance.TestEquals_Byte0x0ff_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int160x0ff_True", new Action( instance.TestEquals_Byte0x0ff_Int160x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int160x1_False", new Action( instance.TestEquals_Byte0x0ff_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int160x80_False", new Action( instance.TestEquals_Byte0x0ff_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int160x8000_False", new Action( instance.TestEquals_Byte0x0ff_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int160xf_False", new Action( instance.TestEquals_Byte0x0ff_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320x0_False", new Action( instance.TestEquals_Byte0x0ff_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320x0ff_True", new Action( instance.TestEquals_Byte0x0ff_Int320x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320x1_False", new Action( instance.TestEquals_Byte0x0ff_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320x80_False", new Action( instance.TestEquals_Byte0x0ff_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320x8000_False", new Action( instance.TestEquals_Byte0x0ff_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320x80000000_False", new Action( instance.TestEquals_Byte0x0ff_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int320xf_False", new Action( instance.TestEquals_Byte0x0ff_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x0_False", new Action( instance.TestEquals_Byte0x0ff_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x0ff_True", new Action( instance.TestEquals_Byte0x0ff_Int640x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x1_False", new Action( instance.TestEquals_Byte0x0ff_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x80_False", new Action( instance.TestEquals_Byte0x0ff_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x8000_False", new Action( instance.TestEquals_Byte0x0ff_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x80000000_False", new Action( instance.TestEquals_Byte0x0ff_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640x8000000000000000_False", new Action( instance.TestEquals_Byte0x0ff_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_Int640xf_False", new Action( instance.TestEquals_Byte0x0ff_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_SByte0x0_False", new Action( instance.TestEquals_Byte0x0ff_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_SByte0x1_False", new Action( instance.TestEquals_Byte0x0ff_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_SByte0x80_False", new Action( instance.TestEquals_Byte0x0ff_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_SByte0xf_False", new Action( instance.TestEquals_Byte0x0ff_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt160x0_False", new Action( instance.TestEquals_Byte0x0ff_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt160x0ff_True", new Action( instance.TestEquals_Byte0x0ff_UInt160x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt160x1_False", new Action( instance.TestEquals_Byte0x0ff_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt320x0_False", new Action( instance.TestEquals_Byte0x0ff_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt320x0ff_True", new Action( instance.TestEquals_Byte0x0ff_UInt320x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt320x1_False", new Action( instance.TestEquals_Byte0x0ff_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt640x0_False", new Action( instance.TestEquals_Byte0x0ff_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt640x0ff_True", new Action( instance.TestEquals_Byte0x0ff_UInt640x0ff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_Byte0x0ff_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x0ff_UInt640x1_False", new Action( instance.TestEquals_Byte0x0ff_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Byte0x0_False", new Action( instance.TestEquals_Byte0x1_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Byte0x0ff_False", new Action( instance.TestEquals_Byte0x1_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Byte0x1_True", new Action( instance.TestEquals_Byte0x1_Byte0x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int160x0_False", new Action( instance.TestEquals_Byte0x1_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int160x0ff_False", new Action( instance.TestEquals_Byte0x1_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int160x1_True", new Action( instance.TestEquals_Byte0x1_Int160x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int160x80_False", new Action( instance.TestEquals_Byte0x1_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int160x8000_False", new Action( instance.TestEquals_Byte0x1_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int160xf_False", new Action( instance.TestEquals_Byte0x1_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320x0_False", new Action( instance.TestEquals_Byte0x1_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320x0ff_False", new Action( instance.TestEquals_Byte0x1_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320x1_True", new Action( instance.TestEquals_Byte0x1_Int320x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320x80_False", new Action( instance.TestEquals_Byte0x1_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320x8000_False", new Action( instance.TestEquals_Byte0x1_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320x80000000_False", new Action( instance.TestEquals_Byte0x1_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int320xf_False", new Action( instance.TestEquals_Byte0x1_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x0_False", new Action( instance.TestEquals_Byte0x1_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x0ff_False", new Action( instance.TestEquals_Byte0x1_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x1_True", new Action( instance.TestEquals_Byte0x1_Int640x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x80_False", new Action( instance.TestEquals_Byte0x1_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x8000_False", new Action( instance.TestEquals_Byte0x1_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x80000000_False", new Action( instance.TestEquals_Byte0x1_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640x8000000000000000_False", new Action( instance.TestEquals_Byte0x1_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_Int640xf_False", new Action( instance.TestEquals_Byte0x1_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_SByte0x0_False", new Action( instance.TestEquals_Byte0x1_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_SByte0x1_True", new Action( instance.TestEquals_Byte0x1_SByte0x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_SByte0x80_False", new Action( instance.TestEquals_Byte0x1_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_SByte0xf_False", new Action( instance.TestEquals_Byte0x1_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt160x0_False", new Action( instance.TestEquals_Byte0x1_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt160x0ff_False", new Action( instance.TestEquals_Byte0x1_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt160x1_True", new Action( instance.TestEquals_Byte0x1_UInt160x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt320x0_False", new Action( instance.TestEquals_Byte0x1_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt320x0ff_False", new Action( instance.TestEquals_Byte0x1_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt320x1_True", new Action( instance.TestEquals_Byte0x1_UInt320x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt640x0_False", new Action( instance.TestEquals_Byte0x1_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt640x0ff_False", new Action( instance.TestEquals_Byte0x1_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_Byte0x1_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Byte0x1_UInt640x1_True", new Action( instance.TestEquals_Byte0x1_UInt640x1_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_DoubleEpsilon_True", new Action( instance.TestEquals_DoubleEpsilon_DoubleEpsilon_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_DoubleMaxValue_False", new Action( instance.TestEquals_DoubleEpsilon_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_DoubleMinValue_False", new Action( instance.TestEquals_DoubleEpsilon_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_DoubleZero_False", new Action( instance.TestEquals_DoubleEpsilon_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_MinusDoubleEpsilon_False", new Action( instance.TestEquals_DoubleEpsilon_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_MinusSingleEpsilon_False", new Action( instance.TestEquals_DoubleEpsilon_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_Nil_False", new Action( instance.TestEquals_DoubleEpsilon_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_SingleEpsilon_False", new Action( instance.TestEquals_DoubleEpsilon_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_SingleMaxValue_False", new Action( instance.TestEquals_DoubleEpsilon_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleEpsilon_SingleMinValue_False", new Action( instance.TestEquals_DoubleEpsilon_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_DoubleEpsilon_False", new Action( instance.TestEquals_DoubleMaxValue_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_DoubleMaxValue_True", new Action( instance.TestEquals_DoubleMaxValue_DoubleMaxValue_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_DoubleMinValue_False", new Action( instance.TestEquals_DoubleMaxValue_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_DoubleZero_False", new Action( instance.TestEquals_DoubleMaxValue_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_MinusDoubleEpsilon_False", new Action( instance.TestEquals_DoubleMaxValue_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_MinusSingleEpsilon_False", new Action( instance.TestEquals_DoubleMaxValue_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_Nil_False", new Action( instance.TestEquals_DoubleMaxValue_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_SingleEpsilon_False", new Action( instance.TestEquals_DoubleMaxValue_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_SingleMaxValue_False", new Action( instance.TestEquals_DoubleMaxValue_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMaxValue_SingleMinValue_False", new Action( instance.TestEquals_DoubleMaxValue_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinusOne_Int16_True", new Action( instance.TestEquals_DoubleMinusOne_Int16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinusOne_Int32_True", new Action( instance.TestEquals_DoubleMinusOne_Int32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinusOne_Int64_True", new Action( instance.TestEquals_DoubleMinusOne_Int64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinusOne_SByte_True", new Action( instance.TestEquals_DoubleMinusOne_SByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_DoubleEpsilon_False", new Action( instance.TestEquals_DoubleMinValue_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_DoubleMaxValue_False", new Action( instance.TestEquals_DoubleMinValue_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_DoubleMinValue_True", new Action( instance.TestEquals_DoubleMinValue_DoubleMinValue_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_DoubleZero_False", new Action( instance.TestEquals_DoubleMinValue_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_MinusDoubleEpsilon_False", new Action( instance.TestEquals_DoubleMinValue_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_MinusSingleEpsilon_False", new Action( instance.TestEquals_DoubleMinValue_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_Nil_False", new Action( instance.TestEquals_DoubleMinValue_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_SingleEpsilon_False", new Action( instance.TestEquals_DoubleMinValue_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_SingleMaxValue_False", new Action( instance.TestEquals_DoubleMinValue_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleMinValue_SingleMinValue_False", new Action( instance.TestEquals_DoubleMinValue_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_Byte_True", new Action( instance.TestEquals_DoubleOne_Byte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_Int16_True", new Action( instance.TestEquals_DoubleOne_Int16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_Int32_True", new Action( instance.TestEquals_DoubleOne_Int32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_Int64_True", new Action( instance.TestEquals_DoubleOne_Int64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_SByte_True", new Action( instance.TestEquals_DoubleOne_SByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_UInt16_True", new Action( instance.TestEquals_DoubleOne_UInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_UInt32_True", new Action( instance.TestEquals_DoubleOne_UInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleOne_UInt64_True", new Action( instance.TestEquals_DoubleOne_UInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_Byte_True", new Action( instance.TestEquals_DoubleZero_Byte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_DoubleEpsilon_False", new Action( instance.TestEquals_DoubleZero_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_DoubleMaxValue_False", new Action( instance.TestEquals_DoubleZero_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_DoubleMinValue_False", new Action( instance.TestEquals_DoubleZero_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_DoubleZero_True", new Action( instance.TestEquals_DoubleZero_DoubleZero_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_Int16_True", new Action( instance.TestEquals_DoubleZero_Int16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_Int32_True", new Action( instance.TestEquals_DoubleZero_Int32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_Int64_True", new Action( instance.TestEquals_DoubleZero_Int64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_MinusDoubleEpsilon_False", new Action( instance.TestEquals_DoubleZero_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_MinusSingleEpsilon_False", new Action( instance.TestEquals_DoubleZero_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_Nil_False", new Action( instance.TestEquals_DoubleZero_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_SByte_True", new Action( instance.TestEquals_DoubleZero_SByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_SingleEpsilon_False", new Action( instance.TestEquals_DoubleZero_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_SingleMaxValue_False", new Action( instance.TestEquals_DoubleZero_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_SingleMinValue_False", new Action( instance.TestEquals_DoubleZero_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_SingleZero_True", new Action( instance.TestEquals_DoubleZero_SingleZero_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_UInt16_True", new Action( instance.TestEquals_DoubleZero_UInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_UInt32_True", new Action( instance.TestEquals_DoubleZero_UInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_DoubleZero_UInt64_True", new Action( instance.TestEquals_DoubleZero_UInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_EmptyArray_Empty_True", new Action( instance.TestEquals_EmptyArray_Empty_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_EmptyArray_Null_False", new Action( instance.TestEquals_EmptyArray_Null_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_EmptyMap_Empty_True", new Action( instance.TestEquals_EmptyMap_Empty_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_EmptyMap_Null_False", new Action( instance.TestEquals_EmptyMap_Null_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_False_False_True", new Action( instance.TestEquals_False_False_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_False_Nil_False", new Action( instance.TestEquals_False_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_False_NonBoolean_False", new Action( instance.TestEquals_False_NonBoolean_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_False_True_False", new Action( instance.TestEquals_False_True_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Byte0x0_False", new Action( instance.TestEquals_Int160x8000_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Byte0x0ff_False", new Action( instance.TestEquals_Int160x8000_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Byte0x1_False", new Action( instance.TestEquals_Int160x8000_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int160x0_False", new Action( instance.TestEquals_Int160x8000_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int160x0ff_False", new Action( instance.TestEquals_Int160x8000_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int160x1_False", new Action( instance.TestEquals_Int160x8000_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int160x80_False", new Action( instance.TestEquals_Int160x8000_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int160x8000_True", new Action( instance.TestEquals_Int160x8000_Int160x8000_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int160xf_False", new Action( instance.TestEquals_Int160x8000_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320x0_False", new Action( instance.TestEquals_Int160x8000_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320x0ff_False", new Action( instance.TestEquals_Int160x8000_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320x1_False", new Action( instance.TestEquals_Int160x8000_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320x80_False", new Action( instance.TestEquals_Int160x8000_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320x8000_True", new Action( instance.TestEquals_Int160x8000_Int320x8000_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320x80000000_False", new Action( instance.TestEquals_Int160x8000_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int320xf_False", new Action( instance.TestEquals_Int160x8000_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x0_False", new Action( instance.TestEquals_Int160x8000_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x0ff_False", new Action( instance.TestEquals_Int160x8000_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x1_False", new Action( instance.TestEquals_Int160x8000_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x80_False", new Action( instance.TestEquals_Int160x8000_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x8000_True", new Action( instance.TestEquals_Int160x8000_Int640x8000_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x80000000_False", new Action( instance.TestEquals_Int160x8000_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640x8000000000000000_False", new Action( instance.TestEquals_Int160x8000_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_Int640xf_False", new Action( instance.TestEquals_Int160x8000_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_SByte0x0_False", new Action( instance.TestEquals_Int160x8000_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_SByte0x1_False", new Action( instance.TestEquals_Int160x8000_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_SByte0x80_False", new Action( instance.TestEquals_Int160x8000_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_SByte0xf_False", new Action( instance.TestEquals_Int160x8000_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt160x0_False", new Action( instance.TestEquals_Int160x8000_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt160x0ff_False", new Action( instance.TestEquals_Int160x8000_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt160x1_False", new Action( instance.TestEquals_Int160x8000_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt320x0_False", new Action( instance.TestEquals_Int160x8000_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt320x0ff_False", new Action( instance.TestEquals_Int160x8000_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt320x1_False", new Action( instance.TestEquals_Int160x8000_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt640x0_False", new Action( instance.TestEquals_Int160x8000_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt640x0ff_False", new Action( instance.TestEquals_Int160x8000_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_Int160x8000_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int160x8000_UInt640x1_False", new Action( instance.TestEquals_Int160x8000_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Byte0x0_False", new Action( instance.TestEquals_Int320x80000000_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Byte0x0ff_False", new Action( instance.TestEquals_Int320x80000000_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Byte0x1_False", new Action( instance.TestEquals_Int320x80000000_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int160x0_False", new Action( instance.TestEquals_Int320x80000000_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int160x0ff_False", new Action( instance.TestEquals_Int320x80000000_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int160x1_False", new Action( instance.TestEquals_Int320x80000000_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int160x80_False", new Action( instance.TestEquals_Int320x80000000_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int160x8000_False", new Action( instance.TestEquals_Int320x80000000_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int160xf_False", new Action( instance.TestEquals_Int320x80000000_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320x0_False", new Action( instance.TestEquals_Int320x80000000_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320x0ff_False", new Action( instance.TestEquals_Int320x80000000_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320x1_False", new Action( instance.TestEquals_Int320x80000000_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320x80_False", new Action( instance.TestEquals_Int320x80000000_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320x8000_False", new Action( instance.TestEquals_Int320x80000000_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320x80000000_True", new Action( instance.TestEquals_Int320x80000000_Int320x80000000_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int320xf_False", new Action( instance.TestEquals_Int320x80000000_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x0_False", new Action( instance.TestEquals_Int320x80000000_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x0ff_False", new Action( instance.TestEquals_Int320x80000000_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x1_False", new Action( instance.TestEquals_Int320x80000000_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x80_False", new Action( instance.TestEquals_Int320x80000000_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x8000_False", new Action( instance.TestEquals_Int320x80000000_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x80000000_True", new Action( instance.TestEquals_Int320x80000000_Int640x80000000_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640x8000000000000000_False", new Action( instance.TestEquals_Int320x80000000_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_Int640xf_False", new Action( instance.TestEquals_Int320x80000000_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_SByte0x0_False", new Action( instance.TestEquals_Int320x80000000_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_SByte0x1_False", new Action( instance.TestEquals_Int320x80000000_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_SByte0x80_False", new Action( instance.TestEquals_Int320x80000000_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_SByte0xf_False", new Action( instance.TestEquals_Int320x80000000_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt160x0_False", new Action( instance.TestEquals_Int320x80000000_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt160x0ff_False", new Action( instance.TestEquals_Int320x80000000_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt160x1_False", new Action( instance.TestEquals_Int320x80000000_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt320x0_False", new Action( instance.TestEquals_Int320x80000000_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt320x0ff_False", new Action( instance.TestEquals_Int320x80000000_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt320x1_False", new Action( instance.TestEquals_Int320x80000000_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt640x0_False", new Action( instance.TestEquals_Int320x80000000_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt640x0ff_False", new Action( instance.TestEquals_Int320x80000000_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_Int320x80000000_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int320x80000000_UInt640x1_False", new Action( instance.TestEquals_Int320x80000000_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Byte0x0_False", new Action( instance.TestEquals_Int640x8000000000000000_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Byte0x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Byte0x1_False", new Action( instance.TestEquals_Int640x8000000000000000_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int160x0_False", new Action( instance.TestEquals_Int640x8000000000000000_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int160x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int160x1_False", new Action( instance.TestEquals_Int640x8000000000000000_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int160x80_False", new Action( instance.TestEquals_Int640x8000000000000000_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int160x8000_False", new Action( instance.TestEquals_Int640x8000000000000000_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int160xf_False", new Action( instance.TestEquals_Int640x8000000000000000_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320x0_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320x1_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320x80_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320x8000_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320x80000000_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int320xf_False", new Action( instance.TestEquals_Int640x8000000000000000_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x0_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x1_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x80_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x8000_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x80000000_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640x8000000000000000_True", new Action( instance.TestEquals_Int640x8000000000000000_Int640x8000000000000000_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_Int640xf_False", new Action( instance.TestEquals_Int640x8000000000000000_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_SByte0x0_False", new Action( instance.TestEquals_Int640x8000000000000000_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_SByte0x1_False", new Action( instance.TestEquals_Int640x8000000000000000_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_SByte0x80_False", new Action( instance.TestEquals_Int640x8000000000000000_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_SByte0xf_False", new Action( instance.TestEquals_Int640x8000000000000000_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt160x0_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt160x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt160x1_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt320x0_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt320x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt320x1_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt640x0_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt640x0ff_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Int640x8000000000000000_UInt640x1_False", new Action( instance.TestEquals_Int640x8000000000000000_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_EqualMap_True", new Action( instance.TestEquals_Map_EqualMap_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_EquivalantMap_True", new Action( instance.TestEquals_Map_EquivalantMap_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_Null_False", new Action( instance.TestEquals_Map_Null_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_Raw_False", new Action( instance.TestEquals_Map_Raw_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_SameLengthButDifferKeyMap_False", new Action( instance.TestEquals_Map_SameLengthButDifferKeyMap_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_SameLengthButDifferValueMap_False", new Action( instance.TestEquals_Map_SameLengthButDifferValueMap_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Map_SubMap_False", new Action( instance.TestEquals_Map_SubMap_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_DoubleEpsilon_False", new Action( instance.TestEquals_MinusDoubleEpsilon_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_DoubleMaxValue_False", new Action( instance.TestEquals_MinusDoubleEpsilon_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_DoubleMinValue_False", new Action( instance.TestEquals_MinusDoubleEpsilon_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_DoubleZero_False", new Action( instance.TestEquals_MinusDoubleEpsilon_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_MinusDoubleEpsilon_True", new Action( instance.TestEquals_MinusDoubleEpsilon_MinusDoubleEpsilon_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_MinusSingleEpsilon_False", new Action( instance.TestEquals_MinusDoubleEpsilon_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_Nil_False", new Action( instance.TestEquals_MinusDoubleEpsilon_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_SingleEpsilon_False", new Action( instance.TestEquals_MinusDoubleEpsilon_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_SingleMaxValue_False", new Action( instance.TestEquals_MinusDoubleEpsilon_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusDoubleEpsilon_SingleMinValue_False", new Action( instance.TestEquals_MinusDoubleEpsilon_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_DoubleEpsilon_False", new Action( instance.TestEquals_MinusSingleEpsilon_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_DoubleMaxValue_False", new Action( instance.TestEquals_MinusSingleEpsilon_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_DoubleMinValue_False", new Action( instance.TestEquals_MinusSingleEpsilon_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_DoubleZero_False", new Action( instance.TestEquals_MinusSingleEpsilon_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_MinusDoubleEpsilon_False", new Action( instance.TestEquals_MinusSingleEpsilon_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_MinusSingleEpsilon_True", new Action( instance.TestEquals_MinusSingleEpsilon_MinusSingleEpsilon_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_Nil_False", new Action( instance.TestEquals_MinusSingleEpsilon_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_SingleEpsilon_False", new Action( instance.TestEquals_MinusSingleEpsilon_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_SingleMaxValue_False", new Action( instance.TestEquals_MinusSingleEpsilon_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_MinusSingleEpsilon_SingleMinValue_False", new Action( instance.TestEquals_MinusSingleEpsilon_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_Nil_True", new Action( instance.TestEquals_Nil_Nil_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NotNil_False", new Action( instance.TestEquals_Nil_NotNil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NullArray_True", new Action( instance.TestEquals_Nil_NullArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NullByteArray_True", new Action( instance.TestEquals_Nil_NullByteArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NullDictionary_True", new Action( instance.TestEquals_Nil_NullDictionary_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NullList_True", new Action( instance.TestEquals_Nil_NullList_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NullMessagePackString_True", new Action( instance.TestEquals_Nil_NullMessagePackString_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_Nil_NullString_True", new Action( instance.TestEquals_Nil_NullString_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_OtherIsNotMessagePackObject_False", new Action( instance.TestEquals_OtherIsNotMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Byte0x0_False", new Action( instance.TestEquals_SByte0x80_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Byte0x0ff_False", new Action( instance.TestEquals_SByte0x80_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Byte0x1_False", new Action( instance.TestEquals_SByte0x80_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int160x0_False", new Action( instance.TestEquals_SByte0x80_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int160x0ff_False", new Action( instance.TestEquals_SByte0x80_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int160x1_False", new Action( instance.TestEquals_SByte0x80_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int160x80_True", new Action( instance.TestEquals_SByte0x80_Int160x80_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int160x8000_False", new Action( instance.TestEquals_SByte0x80_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int160xf_False", new Action( instance.TestEquals_SByte0x80_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320x0_False", new Action( instance.TestEquals_SByte0x80_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320x0ff_False", new Action( instance.TestEquals_SByte0x80_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320x1_False", new Action( instance.TestEquals_SByte0x80_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320x80_True", new Action( instance.TestEquals_SByte0x80_Int320x80_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320x8000_False", new Action( instance.TestEquals_SByte0x80_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320x80000000_False", new Action( instance.TestEquals_SByte0x80_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int320xf_False", new Action( instance.TestEquals_SByte0x80_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x0_False", new Action( instance.TestEquals_SByte0x80_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x0ff_False", new Action( instance.TestEquals_SByte0x80_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x1_False", new Action( instance.TestEquals_SByte0x80_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x80_True", new Action( instance.TestEquals_SByte0x80_Int640x80_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x8000_False", new Action( instance.TestEquals_SByte0x80_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x80000000_False", new Action( instance.TestEquals_SByte0x80_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640x8000000000000000_False", new Action( instance.TestEquals_SByte0x80_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_Int640xf_False", new Action( instance.TestEquals_SByte0x80_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_SByte0x0_False", new Action( instance.TestEquals_SByte0x80_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_SByte0x1_False", new Action( instance.TestEquals_SByte0x80_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_SByte0x80_True", new Action( instance.TestEquals_SByte0x80_SByte0x80_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_SByte0xf_False", new Action( instance.TestEquals_SByte0x80_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt160x0_False", new Action( instance.TestEquals_SByte0x80_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt160x0ff_False", new Action( instance.TestEquals_SByte0x80_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt160x1_False", new Action( instance.TestEquals_SByte0x80_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt320x0_False", new Action( instance.TestEquals_SByte0x80_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt320x0ff_False", new Action( instance.TestEquals_SByte0x80_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt320x1_False", new Action( instance.TestEquals_SByte0x80_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt640x0_False", new Action( instance.TestEquals_SByte0x80_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt640x0ff_False", new Action( instance.TestEquals_SByte0x80_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_SByte0x80_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0x80_UInt640x1_False", new Action( instance.TestEquals_SByte0x80_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Byte0x0_False", new Action( instance.TestEquals_SByte0xf_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Byte0x0ff_False", new Action( instance.TestEquals_SByte0xf_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Byte0x1_False", new Action( instance.TestEquals_SByte0xf_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int160x0_False", new Action( instance.TestEquals_SByte0xf_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int160x0ff_False", new Action( instance.TestEquals_SByte0xf_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int160x1_False", new Action( instance.TestEquals_SByte0xf_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int160x80_False", new Action( instance.TestEquals_SByte0xf_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int160x8000_False", new Action( instance.TestEquals_SByte0xf_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int160xf_True", new Action( instance.TestEquals_SByte0xf_Int160xf_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320x0_False", new Action( instance.TestEquals_SByte0xf_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320x0ff_False", new Action( instance.TestEquals_SByte0xf_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320x1_False", new Action( instance.TestEquals_SByte0xf_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320x80_False", new Action( instance.TestEquals_SByte0xf_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320x8000_False", new Action( instance.TestEquals_SByte0xf_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320x80000000_False", new Action( instance.TestEquals_SByte0xf_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int320xf_True", new Action( instance.TestEquals_SByte0xf_Int320xf_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x0_False", new Action( instance.TestEquals_SByte0xf_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x0ff_False", new Action( instance.TestEquals_SByte0xf_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x1_False", new Action( instance.TestEquals_SByte0xf_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x80_False", new Action( instance.TestEquals_SByte0xf_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x8000_False", new Action( instance.TestEquals_SByte0xf_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x80000000_False", new Action( instance.TestEquals_SByte0xf_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640x8000000000000000_False", new Action( instance.TestEquals_SByte0xf_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_Int640xf_True", new Action( instance.TestEquals_SByte0xf_Int640xf_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_SByte0x0_False", new Action( instance.TestEquals_SByte0xf_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_SByte0x1_False", new Action( instance.TestEquals_SByte0xf_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_SByte0x80_False", new Action( instance.TestEquals_SByte0xf_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_SByte0xf_True", new Action( instance.TestEquals_SByte0xf_SByte0xf_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt160x0_False", new Action( instance.TestEquals_SByte0xf_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt160x0ff_False", new Action( instance.TestEquals_SByte0xf_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt160x1_False", new Action( instance.TestEquals_SByte0xf_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt320x0_False", new Action( instance.TestEquals_SByte0xf_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt320x0ff_False", new Action( instance.TestEquals_SByte0xf_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt320x1_False", new Action( instance.TestEquals_SByte0xf_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt640x0_False", new Action( instance.TestEquals_SByte0xf_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt640x0ff_False", new Action( instance.TestEquals_SByte0xf_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_SByte0xf_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SByte0xf_UInt640x1_False", new Action( instance.TestEquals_SByte0xf_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_DoubleEpsilon_False", new Action( instance.TestEquals_SingleEpsilon_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_DoubleMaxValue_False", new Action( instance.TestEquals_SingleEpsilon_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_DoubleMinValue_False", new Action( instance.TestEquals_SingleEpsilon_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_DoubleZero_False", new Action( instance.TestEquals_SingleEpsilon_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_MinusDoubleEpsilon_False", new Action( instance.TestEquals_SingleEpsilon_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_MinusSingleEpsilon_False", new Action( instance.TestEquals_SingleEpsilon_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_Nil_False", new Action( instance.TestEquals_SingleEpsilon_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_SingleEpsilon_True", new Action( instance.TestEquals_SingleEpsilon_SingleEpsilon_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_SingleMaxValue_False", new Action( instance.TestEquals_SingleEpsilon_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleEpsilon_SingleMinValue_False", new Action( instance.TestEquals_SingleEpsilon_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_DoubleEpsilon_False", new Action( instance.TestEquals_SingleMaxValue_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_DoubleMaxValue_False", new Action( instance.TestEquals_SingleMaxValue_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_DoubleMinValue_False", new Action( instance.TestEquals_SingleMaxValue_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_DoubleZero_False", new Action( instance.TestEquals_SingleMaxValue_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_MinusDoubleEpsilon_False", new Action( instance.TestEquals_SingleMaxValue_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_MinusSingleEpsilon_False", new Action( instance.TestEquals_SingleMaxValue_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_Nil_False", new Action( instance.TestEquals_SingleMaxValue_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_SingleEpsilon_False", new Action( instance.TestEquals_SingleMaxValue_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_SingleMaxValue_True", new Action( instance.TestEquals_SingleMaxValue_SingleMaxValue_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMaxValue_SingleMinValue_False", new Action( instance.TestEquals_SingleMaxValue_SingleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinusOne_Int16_True", new Action( instance.TestEquals_SingleMinusOne_Int16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinusOne_Int32_True", new Action( instance.TestEquals_SingleMinusOne_Int32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinusOne_Int64_True", new Action( instance.TestEquals_SingleMinusOne_Int64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinusOne_SByte_True", new Action( instance.TestEquals_SingleMinusOne_SByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_DoubleEpsilon_False", new Action( instance.TestEquals_SingleMinValue_DoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_DoubleMaxValue_False", new Action( instance.TestEquals_SingleMinValue_DoubleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_DoubleMinValue_False", new Action( instance.TestEquals_SingleMinValue_DoubleMinValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_DoubleZero_False", new Action( instance.TestEquals_SingleMinValue_DoubleZero_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_MinusDoubleEpsilon_False", new Action( instance.TestEquals_SingleMinValue_MinusDoubleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_MinusSingleEpsilon_False", new Action( instance.TestEquals_SingleMinValue_MinusSingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_Nil_False", new Action( instance.TestEquals_SingleMinValue_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_SingleEpsilon_False", new Action( instance.TestEquals_SingleMinValue_SingleEpsilon_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_SingleMaxValue_False", new Action( instance.TestEquals_SingleMinValue_SingleMaxValue_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleMinValue_SingleMinValue_True", new Action( instance.TestEquals_SingleMinValue_SingleMinValue_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_Byte_True", new Action( instance.TestEquals_SingleOne_Byte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_Int16_True", new Action( instance.TestEquals_SingleOne_Int16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_Int32_True", new Action( instance.TestEquals_SingleOne_Int32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_Int64_True", new Action( instance.TestEquals_SingleOne_Int64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_SByte_True", new Action( instance.TestEquals_SingleOne_SByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_UInt16_True", new Action( instance.TestEquals_SingleOne_UInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_UInt32_True", new Action( instance.TestEquals_SingleOne_UInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleOne_UInt64_True", new Action( instance.TestEquals_SingleOne_UInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_Byte_True", new Action( instance.TestEquals_SingleZero_Byte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_Int16_True", new Action( instance.TestEquals_SingleZero_Int16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_Int32_True", new Action( instance.TestEquals_SingleZero_Int32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_Int64_True", new Action( instance.TestEquals_SingleZero_Int64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_SByte_True", new Action( instance.TestEquals_SingleZero_SByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_UInt16_True", new Action( instance.TestEquals_SingleZero_UInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_UInt32_True", new Action( instance.TestEquals_SingleZero_UInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_SingleZero_UInt64_True", new Action( instance.TestEquals_SingleZero_UInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_True_False_False", new Action( instance.TestEquals_True_False_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_True_Nil_False", new Action( instance.TestEquals_True_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_True_NonBoolean_False", new Action( instance.TestEquals_True_NonBoolean_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_True_True_True", new Action( instance.TestEquals_True_True_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Byte0x0_True", new Action( instance.TestEquals_UInt160x0_Byte0x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Byte0x0ff_False", new Action( instance.TestEquals_UInt160x0_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Byte0x1_False", new Action( instance.TestEquals_UInt160x0_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int160x0_True", new Action( instance.TestEquals_UInt160x0_Int160x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int160x0ff_False", new Action( instance.TestEquals_UInt160x0_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int160x1_False", new Action( instance.TestEquals_UInt160x0_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int160x80_False", new Action( instance.TestEquals_UInt160x0_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int160x8000_False", new Action( instance.TestEquals_UInt160x0_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int160xf_False", new Action( instance.TestEquals_UInt160x0_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320x0_True", new Action( instance.TestEquals_UInt160x0_Int320x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320x0ff_False", new Action( instance.TestEquals_UInt160x0_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320x1_False", new Action( instance.TestEquals_UInt160x0_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320x80_False", new Action( instance.TestEquals_UInt160x0_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320x8000_False", new Action( instance.TestEquals_UInt160x0_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320x80000000_False", new Action( instance.TestEquals_UInt160x0_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int320xf_False", new Action( instance.TestEquals_UInt160x0_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x0_True", new Action( instance.TestEquals_UInt160x0_Int640x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x0ff_False", new Action( instance.TestEquals_UInt160x0_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x1_False", new Action( instance.TestEquals_UInt160x0_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x80_False", new Action( instance.TestEquals_UInt160x0_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x8000_False", new Action( instance.TestEquals_UInt160x0_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x80000000_False", new Action( instance.TestEquals_UInt160x0_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640x8000000000000000_False", new Action( instance.TestEquals_UInt160x0_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_Int640xf_False", new Action( instance.TestEquals_UInt160x0_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_SByte0x0_True", new Action( instance.TestEquals_UInt160x0_SByte0x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_SByte0x1_False", new Action( instance.TestEquals_UInt160x0_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_SByte0x80_False", new Action( instance.TestEquals_UInt160x0_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_SByte0xf_False", new Action( instance.TestEquals_UInt160x0_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt160x0_True", new Action( instance.TestEquals_UInt160x0_UInt160x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt160x0ff_False", new Action( instance.TestEquals_UInt160x0_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt160x1_False", new Action( instance.TestEquals_UInt160x0_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt320x0_True", new Action( instance.TestEquals_UInt160x0_UInt320x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt320x0ff_False", new Action( instance.TestEquals_UInt160x0_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt320x1_False", new Action( instance.TestEquals_UInt160x0_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt640x0_True", new Action( instance.TestEquals_UInt160x0_UInt640x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt640x0ff_False", new Action( instance.TestEquals_UInt160x0_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_UInt160x0_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt160x0_UInt640x1_False", new Action( instance.TestEquals_UInt160x0_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Byte0x0_True", new Action( instance.TestEquals_UInt320x0_Byte0x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Byte0x0ff_False", new Action( instance.TestEquals_UInt320x0_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Byte0x1_False", new Action( instance.TestEquals_UInt320x0_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int160x0_True", new Action( instance.TestEquals_UInt320x0_Int160x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int160x0ff_False", new Action( instance.TestEquals_UInt320x0_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int160x1_False", new Action( instance.TestEquals_UInt320x0_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int160x80_False", new Action( instance.TestEquals_UInt320x0_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int160x8000_False", new Action( instance.TestEquals_UInt320x0_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int160xf_False", new Action( instance.TestEquals_UInt320x0_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320x0_True", new Action( instance.TestEquals_UInt320x0_Int320x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320x0ff_False", new Action( instance.TestEquals_UInt320x0_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320x1_False", new Action( instance.TestEquals_UInt320x0_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320x80_False", new Action( instance.TestEquals_UInt320x0_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320x8000_False", new Action( instance.TestEquals_UInt320x0_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320x80000000_False", new Action( instance.TestEquals_UInt320x0_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int320xf_False", new Action( instance.TestEquals_UInt320x0_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x0_True", new Action( instance.TestEquals_UInt320x0_Int640x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x0ff_False", new Action( instance.TestEquals_UInt320x0_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x1_False", new Action( instance.TestEquals_UInt320x0_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x80_False", new Action( instance.TestEquals_UInt320x0_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x8000_False", new Action( instance.TestEquals_UInt320x0_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x80000000_False", new Action( instance.TestEquals_UInt320x0_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640x8000000000000000_False", new Action( instance.TestEquals_UInt320x0_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_Int640xf_False", new Action( instance.TestEquals_UInt320x0_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_SByte0x0_True", new Action( instance.TestEquals_UInt320x0_SByte0x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_SByte0x1_False", new Action( instance.TestEquals_UInt320x0_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_SByte0x80_False", new Action( instance.TestEquals_UInt320x0_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_SByte0xf_False", new Action( instance.TestEquals_UInt320x0_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt160x0_True", new Action( instance.TestEquals_UInt320x0_UInt160x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt160x0ff_False", new Action( instance.TestEquals_UInt320x0_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt160x1_False", new Action( instance.TestEquals_UInt320x0_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt320x0_True", new Action( instance.TestEquals_UInt320x0_UInt320x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt320x0ff_False", new Action( instance.TestEquals_UInt320x0_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt320x1_False", new Action( instance.TestEquals_UInt320x0_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt640x0_True", new Action( instance.TestEquals_UInt320x0_UInt640x0_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt640x0ff_False", new Action( instance.TestEquals_UInt320x0_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt640x0ffffffffffffffff_False", new Action( instance.TestEquals_UInt320x0_UInt640x0ffffffffffffffff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt320x0_UInt640x1_False", new Action( instance.TestEquals_UInt320x0_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Byte0x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Byte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Byte0x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Byte0x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Byte0x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Byte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int160x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int160x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int160x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int160x80_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int160x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int160x8000_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int160x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int160xf_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int160xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320x80_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320x8000_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320x80000000_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int320xf_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int320xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x80_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x8000_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x8000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x80000000_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x80000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640x8000000000000000_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640x8000000000000000_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_Int640xf_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_Int640xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_SByte0x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_SByte0x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_SByte0x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_SByte0x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_SByte0x80_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_SByte0x80_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_SByte0xf_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_SByte0xf_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt160x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt160x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt160x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt160x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt160x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt160x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt320x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt320x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt320x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt320x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt320x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt320x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt640x0_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt640x0_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt640x0ff_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt640x0ff_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt640x0ffffffffffffffff_True", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt640x0ffffffffffffffff_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEquals_UInt640x0ffffffffffffffff_UInt640x1_False", new Action( instance.TestEquals_UInt640x0ffffffffffffffff_UInt640x1_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsByte_Nil_False", new Action( instance.TestEqualsByte_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsBytePlusOne_Double_True", new Action( instance.TestEqualsBytePlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsBytePlusOne_Single_True", new Action( instance.TestEqualsBytePlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsByteZero_Double_True", new Action( instance.TestEqualsByteZero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsByteZero_Single_True", new Action( instance.TestEqualsByteZero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16_Nil_False", new Action( instance.TestEqualsInt16_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16MinusOne_Double_True", new Action( instance.TestEqualsInt16MinusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16MinusOne_Single_True", new Action( instance.TestEqualsInt16MinusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16PlusOne_Double_True", new Action( instance.TestEqualsInt16PlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16PlusOne_Single_True", new Action( instance.TestEqualsInt16PlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16Zero_Double_True", new Action( instance.TestEqualsInt16Zero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt16Zero_Single_True", new Action( instance.TestEqualsInt16Zero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32_Nil_False", new Action( instance.TestEqualsInt32_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32MinusOne_Double_True", new Action( instance.TestEqualsInt32MinusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32MinusOne_Single_True", new Action( instance.TestEqualsInt32MinusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32PlusOne_Double_True", new Action( instance.TestEqualsInt32PlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32PlusOne_Single_True", new Action( instance.TestEqualsInt32PlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32Zero_Double_True", new Action( instance.TestEqualsInt32Zero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt32Zero_Single_True", new Action( instance.TestEqualsInt32Zero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64_Nil_False", new Action( instance.TestEqualsInt64_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64MinusOne_Double_True", new Action( instance.TestEqualsInt64MinusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64MinusOne_Single_True", new Action( instance.TestEqualsInt64MinusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64PlusOne_Double_True", new Action( instance.TestEqualsInt64PlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64PlusOne_Single_True", new Action( instance.TestEqualsInt64PlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64Zero_Double_True", new Action( instance.TestEqualsInt64Zero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsInt64Zero_Single_True", new Action( instance.TestEqualsInt64Zero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSByte_Nil_False", new Action( instance.TestEqualsSByte_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSByteMinusOne_Double_True", new Action( instance.TestEqualsSByteMinusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSByteMinusOne_Single_True", new Action( instance.TestEqualsSByteMinusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSBytePlusOne_Double_True", new Action( instance.TestEqualsSBytePlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSBytePlusOne_Single_True", new Action( instance.TestEqualsSBytePlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSByteZero_Double_True", new Action( instance.TestEqualsSByteZero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsSByteZero_Single_True", new Action( instance.TestEqualsSByteZero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt16_Nil_False", new Action( instance.TestEqualsUInt16_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt16PlusOne_Double_True", new Action( instance.TestEqualsUInt16PlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt16PlusOne_Single_True", new Action( instance.TestEqualsUInt16PlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt16Zero_Double_True", new Action( instance.TestEqualsUInt16Zero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt16Zero_Single_True", new Action( instance.TestEqualsUInt16Zero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt32_Nil_False", new Action( instance.TestEqualsUInt32_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt32PlusOne_Double_True", new Action( instance.TestEqualsUInt32PlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt32PlusOne_Single_True", new Action( instance.TestEqualsUInt32PlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt32Zero_Double_True", new Action( instance.TestEqualsUInt32Zero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt32Zero_Single_True", new Action( instance.TestEqualsUInt32Zero_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt64_Nil_False", new Action( instance.TestEqualsUInt64_Nil_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt64PlusOne_Double_True", new Action( instance.TestEqualsUInt64PlusOne_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt64PlusOne_Single_True", new Action( instance.TestEqualsUInt64PlusOne_Single_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt64Zero_Double_True", new Action( instance.TestEqualsUInt64Zero_Double_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestEqualsUInt64Zero_Single_True", new Action( instance.TestEqualsUInt64Zero_Single_True ) ) );
		}
	} 

	internal static class MessagePackObjectTest_ExceptionalsInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_Exceptionals();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_Exceptionals )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsBinary_Nil_ReturnsNull", new Action( instance.TestAsBinary_Nil_ReturnsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsEnumreable_Nil_ReturnsNull", new Action( instance.TestAsEnumreable_Nil_ReturnsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsEnumreable_String", new Action( instance.TestAsEnumreable_String ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsList_Nil_ReturnsNull", new Action( instance.TestAsList_Nil_ReturnsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsList_String", new Action( instance.TestAsList_String ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString_Nil_ReturnsNull", new Action( instance.TestAsString_Nil_ReturnsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString_NotString", new Action( instance.TestAsString_NotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_NotString", new Action( instance.TestAsStringUtf16_NotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf8_NotString", new Action( instance.TestAsStringUtf8_NotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ForNull_Null", new Action( instance.TestIsTypeOf_ForNull_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Null", new Action( instance.TestIsTypeOf_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitBoolean_Nil_Fail", new Action( instance.TestOpExplicitBoolean_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitBoolean_NotNumerics_Fail", new Action( instance.TestOpExplicitBoolean_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_ByteMaxValue_Success", new Action( instance.TestOpExplicitByte_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_ByteMinValue_Success", new Action( instance.TestOpExplicitByte_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_DoublePlusOne", new Action( instance.TestOpExplicitByte_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Int16MaxValue_Fail", new Action( instance.TestOpExplicitByte_Int16MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Int16MinValue_Fail", new Action( instance.TestOpExplicitByte_Int16MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Int32MaxValue_Fail", new Action( instance.TestOpExplicitByte_Int32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Int32MinValue_Fail", new Action( instance.TestOpExplicitByte_Int32MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Int64MaxValue_Fail", new Action( instance.TestOpExplicitByte_Int64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Int64MinValue_Fail", new Action( instance.TestOpExplicitByte_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_Nil_Fail", new Action( instance.TestOpExplicitByte_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_NotNumerics_Fail", new Action( instance.TestOpExplicitByte_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_SByteMaxValue_Success", new Action( instance.TestOpExplicitByte_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_SByteMinValue_Fail", new Action( instance.TestOpExplicitByte_SByteMinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_SinglePlusOne", new Action( instance.TestOpExplicitByte_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_UInt16MaxValue_Fail", new Action( instance.TestOpExplicitByte_UInt16MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_UInt16MinValue_Success", new Action( instance.TestOpExplicitByte_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_UInt32MaxValue_Fail", new Action( instance.TestOpExplicitByte_UInt32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_UInt32MinValue_Success", new Action( instance.TestOpExplicitByte_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitByte_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitByte_UInt64MinValue_Success", new Action( instance.TestOpExplicitByte_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitDouble_Int64MinValue", new Action( instance.TestOpExplicitDouble_Int64MinValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitDouble_Nil_Fail", new Action( instance.TestOpExplicitDouble_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitDouble_NotNumerics_Fail", new Action( instance.TestOpExplicitDouble_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitDouble_Single_Success", new Action( instance.TestOpExplicitDouble_Single_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitDouble_UInt64MaxValue", new Action( instance.TestOpExplicitDouble_UInt64MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_ByteMaxValue_Success", new Action( instance.TestOpExplicitInt16_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_ByteMinValue_Success", new Action( instance.TestOpExplicitInt16_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_DoubleMinusOne", new Action( instance.TestOpExplicitInt16_DoubleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_DoublePlusOne", new Action( instance.TestOpExplicitInt16_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Int16MaxValue_Success", new Action( instance.TestOpExplicitInt16_Int16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Int16MinValue_Success", new Action( instance.TestOpExplicitInt16_Int16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Int32MaxValue_Fail", new Action( instance.TestOpExplicitInt16_Int32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Int32MinValue_Fail", new Action( instance.TestOpExplicitInt16_Int32MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Int64MaxValue_Fail", new Action( instance.TestOpExplicitInt16_Int64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Int64MinValue_Fail", new Action( instance.TestOpExplicitInt16_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_Nil_Fail", new Action( instance.TestOpExplicitInt16_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_NotNumerics_Fail", new Action( instance.TestOpExplicitInt16_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_SByteMaxValue_Success", new Action( instance.TestOpExplicitInt16_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_SByteMinValue_Success", new Action( instance.TestOpExplicitInt16_SByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_SingleMinusOne", new Action( instance.TestOpExplicitInt16_SingleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_SinglePlusOne", new Action( instance.TestOpExplicitInt16_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_UInt16MaxValue_Fail", new Action( instance.TestOpExplicitInt16_UInt16MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_UInt16MinValue_Success", new Action( instance.TestOpExplicitInt16_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_UInt32MaxValue_Fail", new Action( instance.TestOpExplicitInt16_UInt32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_UInt32MinValue_Success", new Action( instance.TestOpExplicitInt16_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitInt16_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt16_UInt64MinValue_Success", new Action( instance.TestOpExplicitInt16_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_ByteMaxValue_Success", new Action( instance.TestOpExplicitInt32_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_ByteMinValue_Success", new Action( instance.TestOpExplicitInt32_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_DoubleMinusOne", new Action( instance.TestOpExplicitInt32_DoubleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_DoublePlusOne", new Action( instance.TestOpExplicitInt32_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Int16MaxValue_Success", new Action( instance.TestOpExplicitInt32_Int16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Int16MinValue_Success", new Action( instance.TestOpExplicitInt32_Int16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Int32MaxValue_Success", new Action( instance.TestOpExplicitInt32_Int32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Int32MinValue_Success", new Action( instance.TestOpExplicitInt32_Int32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Int64MaxValue_Fail", new Action( instance.TestOpExplicitInt32_Int64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Int64MinValue_Fail", new Action( instance.TestOpExplicitInt32_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_Nil_Fail", new Action( instance.TestOpExplicitInt32_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_NotNumerics_Fail", new Action( instance.TestOpExplicitInt32_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_SByteMaxValue_Success", new Action( instance.TestOpExplicitInt32_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_SByteMinValue_Success", new Action( instance.TestOpExplicitInt32_SByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_SingleMinusOne", new Action( instance.TestOpExplicitInt32_SingleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_SinglePlusOne", new Action( instance.TestOpExplicitInt32_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_UInt16MaxValue_Success", new Action( instance.TestOpExplicitInt32_UInt16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_UInt16MinValue_Success", new Action( instance.TestOpExplicitInt32_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_UInt32MaxValue_Fail", new Action( instance.TestOpExplicitInt32_UInt32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_UInt32MinValue_Success", new Action( instance.TestOpExplicitInt32_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitInt32_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt32_UInt64MinValue_Success", new Action( instance.TestOpExplicitInt32_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_ByteMaxValue_Success", new Action( instance.TestOpExplicitInt64_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_ByteMinValue_Success", new Action( instance.TestOpExplicitInt64_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_DoubleMinusOne", new Action( instance.TestOpExplicitInt64_DoubleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_DoublePlusOne", new Action( instance.TestOpExplicitInt64_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Int16MaxValue_Success", new Action( instance.TestOpExplicitInt64_Int16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Int16MinValue_Success", new Action( instance.TestOpExplicitInt64_Int16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Int32MaxValue_Success", new Action( instance.TestOpExplicitInt64_Int32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Int32MinValue_Success", new Action( instance.TestOpExplicitInt64_Int32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Int64MaxValue_Success", new Action( instance.TestOpExplicitInt64_Int64MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Int64MinValue_Success", new Action( instance.TestOpExplicitInt64_Int64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_Nil_Fail", new Action( instance.TestOpExplicitInt64_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_NotNumerics_Fail", new Action( instance.TestOpExplicitInt64_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_SByteMaxValue_Success", new Action( instance.TestOpExplicitInt64_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_SByteMinValue_Success", new Action( instance.TestOpExplicitInt64_SByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_SingleMinusOne", new Action( instance.TestOpExplicitInt64_SingleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_SinglePlusOne", new Action( instance.TestOpExplicitInt64_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_UInt16MaxValue_Success", new Action( instance.TestOpExplicitInt64_UInt16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_UInt16MinValue_Success", new Action( instance.TestOpExplicitInt64_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_UInt32MaxValue_Success", new Action( instance.TestOpExplicitInt64_UInt32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_UInt32MinValue_Success", new Action( instance.TestOpExplicitInt64_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitInt64_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitInt64_UInt64MinValue_Success", new Action( instance.TestOpExplicitInt64_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_ByteMaxValue_Fail", new Action( instance.TestOpExplicitSByte_ByteMaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_ByteMinValue_Success", new Action( instance.TestOpExplicitSByte_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_DoubleMinusOne", new Action( instance.TestOpExplicitSByte_DoubleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_DoublePlusOne", new Action( instance.TestOpExplicitSByte_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Int16MaxValue_Fail", new Action( instance.TestOpExplicitSByte_Int16MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Int16MinValue_Fail", new Action( instance.TestOpExplicitSByte_Int16MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Int32MaxValue_Fail", new Action( instance.TestOpExplicitSByte_Int32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Int32MinValue_Fail", new Action( instance.TestOpExplicitSByte_Int32MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Int64MaxValue_Fail", new Action( instance.TestOpExplicitSByte_Int64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Int64MinValue_Fail", new Action( instance.TestOpExplicitSByte_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_Nil_Fail", new Action( instance.TestOpExplicitSByte_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_NotNumerics_Fail", new Action( instance.TestOpExplicitSByte_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_SByteMaxValue_Success", new Action( instance.TestOpExplicitSByte_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_SByteMinValue_Success", new Action( instance.TestOpExplicitSByte_SByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_SingleMinusOne", new Action( instance.TestOpExplicitSByte_SingleMinusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_SinglePlusOne", new Action( instance.TestOpExplicitSByte_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_UInt16MaxValue_Fail", new Action( instance.TestOpExplicitSByte_UInt16MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_UInt16MinValue_Success", new Action( instance.TestOpExplicitSByte_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_UInt32MaxValue_Fail", new Action( instance.TestOpExplicitSByte_UInt32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_UInt32MinValue_Success", new Action( instance.TestOpExplicitSByte_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitSByte_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSByte_UInt64MinValue_Success", new Action( instance.TestOpExplicitSByte_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSingle_Double_Success", new Action( instance.TestOpExplicitSingle_Double_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSingle_Int64MinValue", new Action( instance.TestOpExplicitSingle_Int64MinValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSingle_Nil_Fail", new Action( instance.TestOpExplicitSingle_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSingle_NotNumerics_Fail", new Action( instance.TestOpExplicitSingle_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitSingle_UInt64MaxValue", new Action( instance.TestOpExplicitSingle_UInt64MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_ByteMaxValue_Success", new Action( instance.TestOpExplicitUInt16_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_ByteMinValue_Success", new Action( instance.TestOpExplicitUInt16_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_DoublePlusOne", new Action( instance.TestOpExplicitUInt16_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Int16MaxValue_Success", new Action( instance.TestOpExplicitUInt16_Int16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Int16MinValue_Fail", new Action( instance.TestOpExplicitUInt16_Int16MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Int32MaxValue_Fail", new Action( instance.TestOpExplicitUInt16_Int32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Int32MinValue_Fail", new Action( instance.TestOpExplicitUInt16_Int32MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Int64MaxValue_Fail", new Action( instance.TestOpExplicitUInt16_Int64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Int64MinValue_Fail", new Action( instance.TestOpExplicitUInt16_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_Nil_Fail", new Action( instance.TestOpExplicitUInt16_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_NotNumerics_Fail", new Action( instance.TestOpExplicitUInt16_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_SByteMaxValue_Success", new Action( instance.TestOpExplicitUInt16_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_SByteMinValue_Fail", new Action( instance.TestOpExplicitUInt16_SByteMinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_SinglePlusOne", new Action( instance.TestOpExplicitUInt16_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_UInt16MaxValue_Success", new Action( instance.TestOpExplicitUInt16_UInt16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_UInt16MinValue_Success", new Action( instance.TestOpExplicitUInt16_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_UInt32MaxValue_Fail", new Action( instance.TestOpExplicitUInt16_UInt32MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_UInt32MinValue_Success", new Action( instance.TestOpExplicitUInt16_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitUInt16_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt16_UInt64MinValue_Success", new Action( instance.TestOpExplicitUInt16_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_ByteMaxValue_Success", new Action( instance.TestOpExplicitUInt32_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_ByteMinValue_Success", new Action( instance.TestOpExplicitUInt32_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_DoublePlusOne", new Action( instance.TestOpExplicitUInt32_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Int16MaxValue_Success", new Action( instance.TestOpExplicitUInt32_Int16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Int16MinValue_Fail", new Action( instance.TestOpExplicitUInt32_Int16MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Int32MaxValue_Success", new Action( instance.TestOpExplicitUInt32_Int32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Int32MinValue_Fail", new Action( instance.TestOpExplicitUInt32_Int32MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Int64MaxValue_Fail", new Action( instance.TestOpExplicitUInt32_Int64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Int64MinValue_Fail", new Action( instance.TestOpExplicitUInt32_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_Nil_Fail", new Action( instance.TestOpExplicitUInt32_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_NotNumerics_Fail", new Action( instance.TestOpExplicitUInt32_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_SByteMaxValue_Success", new Action( instance.TestOpExplicitUInt32_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_SByteMinValue_Fail", new Action( instance.TestOpExplicitUInt32_SByteMinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_SinglePlusOne", new Action( instance.TestOpExplicitUInt32_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_UInt16MaxValue_Success", new Action( instance.TestOpExplicitUInt32_UInt16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_UInt16MinValue_Success", new Action( instance.TestOpExplicitUInt32_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_UInt32MaxValue_Success", new Action( instance.TestOpExplicitUInt32_UInt32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_UInt32MinValue_Success", new Action( instance.TestOpExplicitUInt32_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_UInt64MaxValue_Fail", new Action( instance.TestOpExplicitUInt32_UInt64MaxValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt32_UInt64MinValue_Success", new Action( instance.TestOpExplicitUInt32_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_ByteMaxValue_Success", new Action( instance.TestOpExplicitUInt64_ByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_ByteMinValue_Success", new Action( instance.TestOpExplicitUInt64_ByteMinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_DoublePlusOne", new Action( instance.TestOpExplicitUInt64_DoublePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Int16MaxValue_Success", new Action( instance.TestOpExplicitUInt64_Int16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Int16MinValue_Fail", new Action( instance.TestOpExplicitUInt64_Int16MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Int32MaxValue_Success", new Action( instance.TestOpExplicitUInt64_Int32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Int32MinValue_Fail", new Action( instance.TestOpExplicitUInt64_Int32MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Int64MaxValue_Success", new Action( instance.TestOpExplicitUInt64_Int64MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Int64MinValue_Fail", new Action( instance.TestOpExplicitUInt64_Int64MinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_Nil_Fail", new Action( instance.TestOpExplicitUInt64_Nil_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_NotNumerics_Fail", new Action( instance.TestOpExplicitUInt64_NotNumerics_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_SByteMaxValue_Success", new Action( instance.TestOpExplicitUInt64_SByteMaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_SByteMinValue_Fail", new Action( instance.TestOpExplicitUInt64_SByteMinValue_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_SinglePlusOne", new Action( instance.TestOpExplicitUInt64_SinglePlusOne ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_UInt16MaxValue_Success", new Action( instance.TestOpExplicitUInt64_UInt16MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_UInt16MinValue_Success", new Action( instance.TestOpExplicitUInt64_UInt16MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_UInt32MaxValue_Success", new Action( instance.TestOpExplicitUInt64_UInt32MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_UInt32MinValue_Success", new Action( instance.TestOpExplicitUInt64_UInt32MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_UInt64MaxValue_Success", new Action( instance.TestOpExplicitUInt64_UInt64MaxValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestOpExplicitUInt64_UInt64MinValue_Success", new Action( instance.TestOpExplicitUInt64_UInt64MinValue_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Null", new Action( instance.TestPackToMessage_Null ) ) );
		}
	} 

	internal static class MessagePackObjectTest_IPackableInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_IPackable();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_IPackable )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Array0x10000Length_Success", new Action( instance.TestPackToMessage_Array0x10000Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Array0xFFFFLength_Success", new Action( instance.TestPackToMessage_Array0xFFFFLength_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Array15Length_Success", new Action( instance.TestPackToMessage_Array15Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Array16Length_Success", new Action( instance.TestPackToMessage_Array16Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Array1Length_Success", new Action( instance.TestPackToMessage_Array1Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_ArrayEmpty_Success", new Action( instance.TestPackToMessage_ArrayEmpty_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Bytes0x10000Length_Success", new Action( instance.TestPackToMessage_Bytes0x10000Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Bytes0xFFFFLength_Success", new Action( instance.TestPackToMessage_Bytes0xFFFFLength_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Bytes1Length_Success", new Action( instance.TestPackToMessage_Bytes1Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Bytes31Length_Success", new Action( instance.TestPackToMessage_Bytes31Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Bytes32Length_Success", new Action( instance.TestPackToMessage_Bytes32Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_BytesEmpty_Success", new Action( instance.TestPackToMessage_BytesEmpty_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Dictionary0x10000Length_Success", new Action( instance.TestPackToMessage_Dictionary0x10000Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Dictionary0xFFFFLength_Success", new Action( instance.TestPackToMessage_Dictionary0xFFFFLength_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Dictionary15Length_Success", new Action( instance.TestPackToMessage_Dictionary15Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Dictionary16Length_Success", new Action( instance.TestPackToMessage_Dictionary16Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Dictionary1Length_Success", new Action( instance.TestPackToMessage_Dictionary1Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_DictionaryEmpty_Success", new Action( instance.TestPackToMessage_DictionaryEmpty_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Double_Success", new Action( instance.TestPackToMessage_Double_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0Length_Success", new Action( instance.TestPackToMessage_Ext0Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x10000Length_Success", new Action( instance.TestPackToMessage_Ext0x10000Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x100Length_Success", new Action( instance.TestPackToMessage_Ext0x100Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x16Length_Success", new Action( instance.TestPackToMessage_Ext0x16Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x17Length_Success", new Action( instance.TestPackToMessage_Ext0x17Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x1Length_ProhibitExtType_InvalidOperationException", new Action( instance.TestPackToMessage_Ext0x1Length_ProhibitExtType_InvalidOperationException ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x1Length_Success", new Action( instance.TestPackToMessage_Ext0x1Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x2Length_Success", new Action( instance.TestPackToMessage_Ext0x2Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x3Length_Success", new Action( instance.TestPackToMessage_Ext0x3Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x4Length_Success", new Action( instance.TestPackToMessage_Ext0x4Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x5Length_Success", new Action( instance.TestPackToMessage_Ext0x5Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x8Length_Success", new Action( instance.TestPackToMessage_Ext0x8Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0x9Length_Success", new Action( instance.TestPackToMessage_Ext0x9Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0xFFFFLength_Success", new Action( instance.TestPackToMessage_Ext0xFFFFLength_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Ext0xFFLength_Success", new Action( instance.TestPackToMessage_Ext0xFFLength_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_False_Success", new Action( instance.TestPackToMessage_False_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Int16_Success", new Action( instance.TestPackToMessage_Int16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Int32_Success", new Action( instance.TestPackToMessage_Int32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Int64_Success", new Action( instance.TestPackToMessage_Int64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Int8_Success", new Action( instance.TestPackToMessage_Int8_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Null_Success", new Action( instance.TestPackToMessage_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Single_Success", new Action( instance.TestPackToMessage_Single_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_String0x10000Length_Success", new Action( instance.TestPackToMessage_String0x10000Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_String0xFFFFLength_Success", new Action( instance.TestPackToMessage_String0xFFFFLength_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_String1Length_Success", new Action( instance.TestPackToMessage_String1Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_String31Length_Success", new Action( instance.TestPackToMessage_String31Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_String32Length_Success", new Action( instance.TestPackToMessage_String32Length_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_StringEmpty_Success", new Action( instance.TestPackToMessage_StringEmpty_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_TinyNegativeIngeter_Success", new Action( instance.TestPackToMessage_TinyNegativeIngeter_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_TinyPositiveInteger_Success", new Action( instance.TestPackToMessage_TinyPositiveInteger_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_True_Success", new Action( instance.TestPackToMessage_True_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_UInt16_Success", new Action( instance.TestPackToMessage_UInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_UInt32_Success", new Action( instance.TestPackToMessage_UInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_UInt64_Success", new Action( instance.TestPackToMessage_UInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_UInt8_Success", new Action( instance.TestPackToMessage_UInt8_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackToMessage_Zero_Success", new Action( instance.TestPackToMessage_Zero_Success ) ) );
		}
	} 

	internal static class MessagePackObjectTest_IsTypeOfInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_IsTypeOf();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_IsTypeOf )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsArray_ArrayEmptyNotNull_True", new Action( instance.TestIsArray_ArrayEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsArray_ArrayNotNull_True", new Action( instance.TestIsArray_ArrayNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsArray_ArrayNull_False", new Action( instance.TestIsArray_ArrayNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsArray_ListEmptyNotNull_True", new Action( instance.TestIsArray_ListEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsArray_ListNotNull_True", new Action( instance.TestIsArray_ListNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsArray_ListNull_False", new Action( instance.TestIsArray_ListNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsList_ArrayEmptyNotNull_True", new Action( instance.TestIsList_ArrayEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsList_ArrayNotNull_True", new Action( instance.TestIsList_ArrayNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsList_ArrayNull_False", new Action( instance.TestIsList_ArrayNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsList_ListEmptyNotNull_True", new Action( instance.TestIsList_ListEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsList_ListNotNull_True", new Action( instance.TestIsList_ListNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsList_ListNull_False", new Action( instance.TestIsList_ListNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsMap_DictionaryEmptyNotNull_True", new Action( instance.TestIsMap_DictionaryEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsMap_DictionaryNotNull_True", new Action( instance.TestIsMap_DictionaryNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsMap_DictionaryNull_False", new Action( instance.TestIsMap_DictionaryNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_ByteArrayEmptyNotNull_True", new Action( instance.TestIsRaw_ByteArrayEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_ByteArrayNotNull_True", new Action( instance.TestIsRaw_ByteArrayNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_ByteArrayNull_False", new Action( instance.TestIsRaw_ByteArrayNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_NonStringBinary_True", new Action( instance.TestIsRaw_NonStringBinary_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_StringEmptyNotNull_True", new Action( instance.TestIsRaw_StringEmptyNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_StringNotNull_True", new Action( instance.TestIsRaw_StringNotNull_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsRaw_StringNull_False", new Action( instance.TestIsRaw_StringNull_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfArrayOfItemType_False", new Action( instance.TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfArrayOfItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfArrayOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfArrayOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfArrayOfNotItemType_False", new Action( instance.TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfArrayOfNotItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfIEnumerableOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfIEnumerableOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfIListOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfIListOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_ArrayEmptyNotNull_IsTypeOfListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNotNull_IsTypeOfArrayOfItemType_False", new Action( instance.TestIsTypeOf_ArrayNotNull_IsTypeOfArrayOfItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNotNull_IsTypeOfArrayOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ArrayNotNull_IsTypeOfArrayOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNotNull_IsTypeOfArrayOfNotItemType_False", new Action( instance.TestIsTypeOf_ArrayNotNull_IsTypeOfArrayOfNotItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNotNull_IsTypeOfIEnumerableOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ArrayNotNull_IsTypeOfIEnumerableOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNotNull_IsTypeOfIListOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ArrayNotNull_IsTypeOfIListOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNotNull_IsTypeOfListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_ArrayNotNull_IsTypeOfListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNull_IsTypeOfArrayOfItemType_Null", new Action( instance.TestIsTypeOf_ArrayNull_IsTypeOfArrayOfItemType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNull_IsTypeOfArrayOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ArrayNull_IsTypeOfArrayOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNull_IsTypeOfArrayOfNotItemType_Null", new Action( instance.TestIsTypeOf_ArrayNull_IsTypeOfArrayOfNotItemType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNull_IsTypeOfIEnumerableOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ArrayNull_IsTypeOfIEnumerableOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNull_IsTypeOfIListOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ArrayNull_IsTypeOfIListOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ArrayNull_IsTypeOfListOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ArrayNull_IsTypeOfListOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Binary_IsTypeOfMessagePackExtendedTypeObject_False", new Action( instance.TestIsTypeOf_Binary_IsTypeOfMessagePackExtendedTypeObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfArrayOfMessagePackObject_False", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfArrayOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfArrayOfNotByteType_False", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfArrayOfNotByteType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfByteArray_True", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfByteArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfCharArray_False", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfCharArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIEnumerableOfByte_True", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIEnumerableOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIEnumerableOfChar_True", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIEnumerableOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIListOfByte_True", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIListOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIListOfChar_True", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfIListOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfListOfByte_False", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfListOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfListOfChar_False", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfListOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfString_True", new Action( instance.TestIsTypeOf_ByteArrayEmptyNotNull_IsTypeOfString_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfArrayOfMessagePackObject_False", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfArrayOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfArrayOfNotByteType_False", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfArrayOfNotByteType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfByteArray_True", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfByteArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfCharArray_False", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfCharArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfIEnumerableOfByte_True", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfIEnumerableOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfIEnumerableOfChar_True", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfIEnumerableOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfIListOfByte_True", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfIListOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfIListOfChar_True", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfIListOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfListOfByte_False", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfListOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfListOfChar_False", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfListOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNotNull_IsTypeOfString_True", new Action( instance.TestIsTypeOf_ByteArrayNotNull_IsTypeOfString_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfArrayOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfArrayOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfArrayOfNotByteType_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfArrayOfNotByteType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfByteArray_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfByteArray_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfCharArray_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfCharArray_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfIEnumerableOfByte_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfIEnumerableOfByte_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfIEnumerableOfChar_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfIEnumerableOfChar_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfIListOfByte_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfIListOfByte_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfIListOfChar_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfIListOfChar_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfListOfByte_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfListOfByte_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfListOfChar_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfListOfChar_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteArrayNull_IsTypeOfString_Null", new Action( instance.TestIsTypeOf_ByteArrayNull_IsTypeOfString_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_ByteByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_BytePlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_BytePlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ByteZero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_ByteZero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfArrayOfItemType_False", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfArrayOfItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfArrayOfMessagePackObject_False", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfArrayOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfArrayOfNotItemType_False", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfArrayOfNotItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfIDictionaryOfMessagePackObject_True", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfIDictionaryOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfIEnumerableOfMessagePackObject_True", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfIEnumerableOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfIListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfIListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfMessagePackObjectDictionary_True", new Action( instance.TestIsTypeOf_DictionaryEmptyNotNull_IsTypeOfMessagePackObjectDictionary_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfArrayOfItemType_False", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfArrayOfItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfArrayOfMessagePackObject_False", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfArrayOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfArrayOfNotItemType_False", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfArrayOfNotItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfIDictionaryOfMessagePackObject_True", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfIDictionaryOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfIEnumerableOfMessagePackObject_True", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfIEnumerableOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfIListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfIListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNotNull_IsTypeOfMessagePackObjectDictionary_True", new Action( instance.TestIsTypeOf_DictionaryNotNull_IsTypeOfMessagePackObjectDictionary_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfArrayOfItemType_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfArrayOfItemType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfArrayOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfArrayOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfArrayOfNotItemType_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfArrayOfNotItemType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfIDictionaryOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfIDictionaryOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfIEnumerableOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfIEnumerableOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfIListOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfIListOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfListOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfListOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_DictionaryNull_IsTypeOfMessagePackObjectDictionary_Null", new Action( instance.TestIsTypeOf_DictionaryNull_IsTypeOfMessagePackObjectDictionary_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Double_IsTypeOfDouble_True", new Action( instance.TestIsTypeOf_Double_IsTypeOfDouble_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Double_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_Double_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Double_IsTypeOfSingle_False", new Action( instance.TestIsTypeOf_Double_IsTypeOfSingle_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16ByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int16ByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Int16MinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int16Int16MinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16MinusOne_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int16MinusOne_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16PlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int16PlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16SByteMinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int16SByteMinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int16Zero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int16Zero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32ByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int32ByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int16MinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int32Int16MinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Int32MinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int32Int32MinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32MinusOne_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int32MinusOne_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32PlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int32PlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32SByteMinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int32SByteMinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int32UInt16MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int32Zero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int32Zero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64ByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int64ByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int16MinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int64Int16MinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int32MinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int64Int32MinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Int64MinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int64Int64MinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64MinusOne_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int64MinusOne_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64PlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int64PlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64SByteMinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_Int64SByteMinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int64UInt16MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int64UInt32MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Int64Zero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_Int64Zero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListEmptyNotNull_IsTypeOfArrayOfItemType_False", new Action( instance.TestIsTypeOf_ListEmptyNotNull_IsTypeOfArrayOfItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListEmptyNotNull_IsTypeOfArrayOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ListEmptyNotNull_IsTypeOfArrayOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListEmptyNotNull_IsTypeOfArrayOfNotItemType_False", new Action( instance.TestIsTypeOf_ListEmptyNotNull_IsTypeOfArrayOfNotItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListEmptyNotNull_IsTypeOfIEnumerableOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ListEmptyNotNull_IsTypeOfIEnumerableOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListEmptyNotNull_IsTypeOfIListOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ListEmptyNotNull_IsTypeOfIListOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListEmptyNotNull_IsTypeOfListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_ListEmptyNotNull_IsTypeOfListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNotNull_IsTypeOfArrayOfItemType_False", new Action( instance.TestIsTypeOf_ListNotNull_IsTypeOfArrayOfItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNotNull_IsTypeOfArrayOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ListNotNull_IsTypeOfArrayOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNotNull_IsTypeOfArrayOfNotItemType_False", new Action( instance.TestIsTypeOf_ListNotNull_IsTypeOfArrayOfNotItemType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNotNull_IsTypeOfIEnumerableOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ListNotNull_IsTypeOfIEnumerableOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNotNull_IsTypeOfIListOfMessagePackObject_True", new Action( instance.TestIsTypeOf_ListNotNull_IsTypeOfIListOfMessagePackObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNotNull_IsTypeOfListOfMessagePackObject_False", new Action( instance.TestIsTypeOf_ListNotNull_IsTypeOfListOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNull_IsTypeOfArrayOfItemType_Null", new Action( instance.TestIsTypeOf_ListNull_IsTypeOfArrayOfItemType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNull_IsTypeOfArrayOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ListNull_IsTypeOfArrayOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNull_IsTypeOfArrayOfNotItemType_Null", new Action( instance.TestIsTypeOf_ListNull_IsTypeOfArrayOfNotItemType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNull_IsTypeOfIEnumerableOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ListNull_IsTypeOfIEnumerableOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNull_IsTypeOfIListOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ListNull_IsTypeOfIListOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_ListNull_IsTypeOfListOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_ListNull_IsTypeOfListOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_MessagePackExtendedTypeObject_IsTypeOfMessagePackExtendedTypeObject_True", new Action( instance.TestIsTypeOf_MessagePackExtendedTypeObject_IsTypeOfMessagePackExtendedTypeObject_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_Byte_False", new Action( instance.TestIsTypeOf_Nil_Byte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_Int16_False", new Action( instance.TestIsTypeOf_Nil_Int16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_Int32_False", new Action( instance.TestIsTypeOf_Nil_Int32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_Int64_False", new Action( instance.TestIsTypeOf_Nil_Int64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_IsTypeOfDouble_False", new Action( instance.TestIsTypeOf_Nil_IsTypeOfDouble_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_IsTypeOfMessagePackExtendedTypeObject_False", new Action( instance.TestIsTypeOf_Nil_IsTypeOfMessagePackExtendedTypeObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_IsTypeOfSingle_False", new Action( instance.TestIsTypeOf_Nil_IsTypeOfSingle_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_SByte_False", new Action( instance.TestIsTypeOf_Nil_SByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_UInt16_False", new Action( instance.TestIsTypeOf_Nil_UInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_UInt32_False", new Action( instance.TestIsTypeOf_Nil_UInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Nil_UInt64_False", new Action( instance.TestIsTypeOf_Nil_UInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfByteArray_True", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfByteArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfCharArray_False", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfCharArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfIEnumerableOfByte_True", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfIEnumerableOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfIEnumerableOfChar_False", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfIEnumerableOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfIListOfByte_True", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfIListOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfIListOfChar_False", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfIListOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfListOfByte_False", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfListOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfListOfChar_False", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfListOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_NonStringBinary_IsTypeOfString_False", new Action( instance.TestIsTypeOf_NonStringBinary_IsTypeOfString_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteMinusOne_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_SByteMinusOne_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SBytePlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_SBytePlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteSByteMinValue_IsTypeOfUInt64_False", new Action( instance.TestIsTypeOf_SByteSByteMinValue_IsTypeOfUInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_SByteZero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_SByteZero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Single_IsTypeOfDouble_True", new Action( instance.TestIsTypeOf_Single_IsTypeOfDouble_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Single_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_Single_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_Single_IsTypeOfSingle_True", new Action( instance.TestIsTypeOf_Single_IsTypeOfSingle_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfArrayOfMessagePackObject_False", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfArrayOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfArrayOfNotByteType_False", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfArrayOfNotByteType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfByteArray_True", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfByteArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfCharArray_False", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfCharArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfIEnumerableOfByte_True", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfIEnumerableOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfIEnumerableOfChar_True", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfIEnumerableOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfIListOfByte_True", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfIListOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfIListOfChar_True", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfIListOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfListOfByte_False", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfListOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfListOfChar_False", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfListOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringEmptyNotNull_IsTypeOfString_True", new Action( instance.TestIsTypeOf_StringEmptyNotNull_IsTypeOfString_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfArrayOfMessagePackObject_False", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfArrayOfMessagePackObject_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfArrayOfNotByteType_False", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfArrayOfNotByteType_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfByteArray_True", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfByteArray_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfCharArray_False", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfCharArray_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfIEnumerableOfByte_True", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfIEnumerableOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfIEnumerableOfChar_True", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfIEnumerableOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfIListOfByte_True", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfIListOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfIListOfChar_True", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfIListOfChar_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfListOfByte_False", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfListOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfListOfChar_False", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfListOfChar_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNotNull_IsTypeOfString_True", new Action( instance.TestIsTypeOf_StringNotNull_IsTypeOfString_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfArrayOfMessagePackObject_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfArrayOfMessagePackObject_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfArrayOfNotByteType_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfArrayOfNotByteType_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfByteArray_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfByteArray_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfCharArray_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfCharArray_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfIEnumerableOfByte_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfIEnumerableOfByte_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfIEnumerableOfChar_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfIEnumerableOfChar_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfIListOfByte_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfIListOfByte_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfIListOfChar_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfIListOfChar_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfListOfByte_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfListOfByte_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfListOfChar_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfListOfChar_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_StringNull_IsTypeOfString_Null", new Action( instance.TestIsTypeOf_StringNull_IsTypeOfString_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt16ByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16PlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt16PlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt16UInt16MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt16Zero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt16Zero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt32ByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32PlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt32PlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt32UInt16MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt32UInt32MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt32Zero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt32Zero_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt64ByteMaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64PlusOne_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt64PlusOne_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt64UInt16MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt64UInt32MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfByte_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfInt16_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfInt32_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfInt64_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfInt64_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfSByte_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfSByte_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfUInt16_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfUInt16_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfUInt32_False", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfUInt32_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt64UInt64MaxValue_IsTypeOfUInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfByte_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfInt16_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfInt32_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfInt64_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfInt64_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfSByte_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfSByte_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfUInt16_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfUInt16_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfUInt32_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfUInt32_True ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIsTypeOf_UInt64Zero_IsTypeOfUInt64_True", new Action( instance.TestIsTypeOf_UInt64Zero_IsTypeOfUInt64_True ) ) );
		}
	} 

	internal static class MessagePackObjectTest_MiscsInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_Miscs();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_Miscs )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode_AllPossibleTypes_Success", new Action( instance.TestGetHashCode_AllPossibleTypes_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_AllPossibleTypes_Success", new Action( instance.TestToString_AllPossibleTypes_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_Binary_Hex", new Action( instance.TestToString_Binary_Hex ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_ExtendedTypeObject_AsIs", new Action( instance.TestToString_ExtendedTypeObject_AsIs ) ) );
		}
	} 

	internal static class MessagePackObjectTest_ObjectInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_Object();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_Object )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Array_Success", new Action( instance.TestFromObject_Array_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Bytes_Success", new Action( instance.TestFromObject_Bytes_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_ByteSequence_Success", new Action( instance.TestFromObject_ByteSequence_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_CharSequence_Success", new Action( instance.TestFromObject_CharSequence_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Dictionary_Success", new Action( instance.TestFromObject_Dictionary_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Double_Success", new Action( instance.TestFromObject_Double_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_ExtendedTypeObject_Success", new Action( instance.TestFromObject_ExtendedTypeObject_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_False_Success", new Action( instance.TestFromObject_False_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Int16_Success", new Action( instance.TestFromObject_Int16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Int32_Success", new Action( instance.TestFromObject_Int32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Int64_Success", new Action( instance.TestFromObject_Int64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Int8_Success", new Action( instance.TestFromObject_Int8_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_MessagePackObject_Success", new Action( instance.TestFromObject_MessagePackObject_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_MessagePackObjectSequence_Success", new Action( instance.TestFromObject_MessagePackObjectSequence_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NotSupported", new Action( instance.TestFromObject_NotSupported ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Null_Success", new Action( instance.TestFromObject_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableDoubleNotNull_Success", new Action( instance.TestFromObject_NullableDoubleNotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableDoubleNull_Nil", new Action( instance.TestFromObject_NullableDoubleNull_Nil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt16NotNull_Success", new Action( instance.TestFromObject_NullableInt16NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt16Null_IsNil", new Action( instance.TestFromObject_NullableInt16Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt32NotNull_Success", new Action( instance.TestFromObject_NullableInt32NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt32Null_IsNil", new Action( instance.TestFromObject_NullableInt32Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt64NotNull_Success", new Action( instance.TestFromObject_NullableInt64NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt64Null_IsNil", new Action( instance.TestFromObject_NullableInt64Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt8NotNull_Success", new Action( instance.TestFromObject_NullableInt8NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableInt8Null_IsNil", new Action( instance.TestFromObject_NullableInt8Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableSingleNotNull_Success", new Action( instance.TestFromObject_NullableSingleNotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableSingleNull_IsNil", new Action( instance.TestFromObject_NullableSingleNull_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt16NotNull_Success", new Action( instance.TestFromObject_NullableUInt16NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt16Null_IsNil", new Action( instance.TestFromObject_NullableUInt16Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt32NotNull_Success", new Action( instance.TestFromObject_NullableUInt32NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt32Null_IsNil", new Action( instance.TestFromObject_NullableUInt32Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt64NotNull_Success", new Action( instance.TestFromObject_NullableUInt64NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt64Null_IsNil", new Action( instance.TestFromObject_NullableUInt64Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt8NotNull_Success", new Action( instance.TestFromObject_NullableUInt8NotNull_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_NullableUInt8Null_IsNil", new Action( instance.TestFromObject_NullableUInt8Null_IsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Single_Success", new Action( instance.TestFromObject_Single_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_String_Success", new Action( instance.TestFromObject_String_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_TinyNegativeIngeter_Success", new Action( instance.TestFromObject_TinyNegativeIngeter_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_TinyPositiveInteger_Success", new Action( instance.TestFromObject_TinyPositiveInteger_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_True_Success", new Action( instance.TestFromObject_True_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_UInt16_Success", new Action( instance.TestFromObject_UInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_UInt32_Success", new Action( instance.TestFromObject_UInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_UInt64_Success", new Action( instance.TestFromObject_UInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_UInt8_Success", new Action( instance.TestFromObject_UInt8_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFromObject_Zero_Success", new Action( instance.TestFromObject_Zero_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Array_Success", new Action( instance.TestToObject_Array_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Bytes_Success", new Action( instance.TestToObject_Bytes_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Dictionary_Success", new Action( instance.TestToObject_Dictionary_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Double_Success", new Action( instance.TestToObject_Double_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_ExtendedTypeObject_Success", new Action( instance.TestToObject_ExtendedTypeObject_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_False_Success", new Action( instance.TestToObject_False_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Int16_Success", new Action( instance.TestToObject_Int16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Int32_Success", new Action( instance.TestToObject_Int32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Int64_Success", new Action( instance.TestToObject_Int64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Int8_Success", new Action( instance.TestToObject_Int8_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_MessagePackObject_Success", new Action( instance.TestToObject_MessagePackObject_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Null_Success", new Action( instance.TestToObject_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Single_Success", new Action( instance.TestToObject_Single_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_String_Success", new Action( instance.TestToObject_String_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_TinyNegativeIngeter_Success", new Action( instance.TestToObject_TinyNegativeIngeter_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_TinyPositiveInteger_Success", new Action( instance.TestToObject_TinyPositiveInteger_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_True_Success", new Action( instance.TestToObject_True_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_UInt16_Success", new Action( instance.TestToObject_UInt16_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_UInt32_Success", new Action( instance.TestToObject_UInt32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_UInt64_Success", new Action( instance.TestToObject_UInt64_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_UInt8_Success", new Action( instance.TestToObject_UInt8_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToObject_Zero_Success", new Action( instance.TestToObject_Zero_Success ) ) );
		}
	} 

	internal static class MessagePackObjectTest_StringInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackObjectTest_String();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackObjectTest_String )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString_EncodingMissmatch", new Action( instance.TestAsString_EncodingMissmatch ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString_IsNotString", new Action( instance.TestAsString_IsNotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString_Null_Success", new Action( instance.TestAsString_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_EncodingIsNotUtf32_SpecifyUtf32_Fail", new Action( instance.TestAsString1_EncodingIsNotUtf32_SpecifyUtf32_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_EncodingIsNull", new Action( instance.TestAsString1_EncodingIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_EncodingIsUtf32_SpecifyNotUtf32_Fail", new Action( instance.TestAsString1_EncodingIsUtf32_SpecifyNotUtf32_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_EncodingIsUtf32_SpecifyUtf32_Success", new Action( instance.TestAsString1_EncodingIsUtf32_SpecifyUtf32_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_EncodingMissmatchAndReturnsNull_Null", new Action( instance.TestAsString1_EncodingMissmatchAndReturnsNull_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_EncodingMissmatchAndThrowsDecoderFallbackException", new Action( instance.TestAsString1_EncodingMissmatchAndThrowsDecoderFallbackException ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_IsNotString", new Action( instance.TestAsString1_IsNotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsString1_Null_Success", new Action( instance.TestAsString1_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_Empty_Success", new Action( instance.TestAsStringUtf16_Empty_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_EncodingMissmatch", new Action( instance.TestAsStringUtf16_EncodingMissmatch ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_ForNonEncoded_Success", new Action( instance.TestAsStringUtf16_ForNonEncoded_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_IsNotString", new Action( instance.TestAsStringUtf16_IsNotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_NonStringBinary", new Action( instance.TestAsStringUtf16_NonStringBinary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_Null_Success", new Action( instance.TestAsStringUtf16_Null_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_Utf16BEWithBom_Success", new Action( instance.TestAsStringUtf16_Utf16BEWithBom_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_Utf16BEWithoutBom_Success", new Action( instance.TestAsStringUtf16_Utf16BEWithoutBom_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_Utf16LEWithBom_Success", new Action( instance.TestAsStringUtf16_Utf16LEWithBom_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf16_Utf16LEWithoutBom_CannotDetected", new Action( instance.TestAsStringUtf16_Utf16LEWithoutBom_CannotDetected ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf8_Empty_Success", new Action( instance.TestAsStringUtf8_Empty_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf8_EncodingMissmatch", new Action( instance.TestAsStringUtf8_EncodingMissmatch ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf8_IsNotString", new Action( instance.TestAsStringUtf8_IsNotString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf8_Normal_Success", new Action( instance.TestAsStringUtf8_Normal_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestAsStringUtf8_Null_Success", new Action( instance.TestAsStringUtf8_Null_Success ) ) );
		}
	} 

	internal static class MessagePackSerializerTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackSerializerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackSerializerTest )testFixtureInstance );
testClassInstance.TestCleanup = new Action( instance.TearDown );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_WithContext_ContextIsNull_Fail", new Action( instance.TestCreate_WithContext_ContextIsNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_WithContext_SameTypeAsCreate1", new Action( instance.TestCreate_WithContext_SameTypeAsCreate1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_WithContext_TypeIsNull_Fail", new Action( instance.TestCreate_WithContext_TypeIsNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_WithoutContext_SameTypeAsCreate1", new Action( instance.TestCreate_WithoutContext_SameTypeAsCreate1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_WithoutContext_TypeIsNull_Fail", new Action( instance.TestCreate_WithoutContext_TypeIsNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate1_WithContext_Null_Fail", new Action( instance.TestCreate1_WithContext_Null_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithContext_ContextIsNull_Fail", new Action( instance.TestGet_WithContext_ContextIsNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithContext_Ok", new Action( instance.TestGet_WithContext_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithContext_SameTypeAsGet1", new Action( instance.TestGet_WithContext_SameTypeAsGet1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithContext_TypeIsNull_Fail", new Action( instance.TestGet_WithContext_TypeIsNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithoutContext_Ok", new Action( instance.TestGet_WithoutContext_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithoutContext_SameTypeAsGet1", new Action( instance.TestGet_WithoutContext_SameTypeAsGet1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet_WithoutContext_TypeIsNull_Fail", new Action( instance.TestGet_WithoutContext_TypeIsNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet1_WithContext_Null_Fail", new Action( instance.TestGet1_WithContext_Null_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet1_WithContext_Ok", new Action( instance.TestGet1_WithContext_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGet1_WithoutContext_Ok", new Action( instance.TestGet1_WithoutContext_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject", new Action( instance.TestUnpackObject ) ) );
		}
	} 

	internal static class MessagePackSerializerTTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackSerializerTTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackSerializerTTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerPackTo_ObjectTreeIsNull_NullableValueType_AsNil", new Action( instance.TestIMessagePackSerializerPackTo_ObjectTreeIsNull_NullableValueType_AsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerPackTo_ObjectTreeIsNull_ReferenceType_AsNil", new Action( instance.TestIMessagePackSerializerPackTo_ObjectTreeIsNull_ReferenceType_AsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerPackTo_ObjectTreeIsNull_ValueType_AsNil", new Action( instance.TestIMessagePackSerializerPackTo_ObjectTreeIsNull_ValueType_AsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerPackTo_ObjectTreeIsOtherType", new Action( instance.TestIMessagePackSerializerPackTo_ObjectTreeIsOtherType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerPackTo_PackerIsNull", new Action( instance.TestIMessagePackSerializerPackTo_PackerIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerPackTo_Valid_Success", new Action( instance.TestIMessagePackSerializerPackTo_Valid_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackFrom_Invalid", new Action( instance.TestIMessagePackSerializerUnpackFrom_Invalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackFrom_UnpackerIsNull", new Action( instance.TestIMessagePackSerializerUnpackFrom_UnpackerIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackFrom_Valid_Success", new Action( instance.TestIMessagePackSerializerUnpackFrom_Valid_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackTo_CollectionIsNull", new Action( instance.TestIMessagePackSerializerUnpackTo_CollectionIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackTo_CollectionTypeIsInvalid", new Action( instance.TestIMessagePackSerializerUnpackTo_CollectionTypeIsInvalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackTo_StreamContainsNull", new Action( instance.TestIMessagePackSerializerUnpackTo_StreamContainsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackTo_StreamContentIsInvalid", new Action( instance.TestIMessagePackSerializerUnpackTo_StreamContentIsInvalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackTo_UnpackerIsNull", new Action( instance.TestIMessagePackSerializerUnpackTo_UnpackerIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSerializerUnpackTo_Valid_Success", new Action( instance.TestIMessagePackSerializerUnpackTo_Valid_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSingleObjectSerializer_PackSingleObject_InvalidType_Fail", new Action( instance.TestIMessagePackSingleObjectSerializer_PackSingleObject_InvalidType_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSingleObjectSerializer_PackSingleObject_Normal_Success", new Action( instance.TestIMessagePackSingleObjectSerializer_PackSingleObject_Normal_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSingleObjectSerializer_PackSingleObject_ReferenceTypeNull_AsNil", new Action( instance.TestIMessagePackSingleObjectSerializer_PackSingleObject_ReferenceTypeNull_AsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSingleObjectSerializer_PackSingleObject_ValueTypeButNull_Fail", new Action( instance.TestIMessagePackSingleObjectSerializer_PackSingleObject_ValueTypeButNull_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIMessagePackSingleObjectSerializer_UnpackSingleObject_Success", new Action( instance.TestIMessagePackSingleObjectSerializer_UnpackSingleObject_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue10_Empty_Reader", new Action( instance.TestIssue10_Empty_Reader ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue10_Empty_ReadString", new Action( instance.TestIssue10_Empty_ReadString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue10_Null_Reader", new Action( instance.TestIssue10_Null_Reader ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue10_Null_ReadString", new Action( instance.TestIssue10_Null_ReadString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue13_ListAsMpo", new Action( instance.TestIssue13_ListAsMpo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue13_MapAsMpo", new Action( instance.TestIssue13_MapAsMpo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue13_StringListMapAsMpoDictionary", new Action( instance.TestIssue13_StringListMapAsMpoDictionary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue28", new Action( instance.TestIssue28 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue41", new Action( instance.TestIssue41 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_StreamIsNull", new Action( instance.TestPack_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackSingleObject_Normal_Success", new Action( instance.TestPackSingleObject_Normal_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPackTo_PackerIsNull", new Action( instance.TestPackTo_PackerIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_StreamIsNull", new Action( instance.TestUnpack_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFrom_StreamIsEmpty", new Action( instance.TestUnpackFrom_StreamIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFrom_StreamIsNullButTypeIsValueType", new Action( instance.TestUnpackFrom_StreamIsNullButTypeIsValueType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFrom_UnpackerIsNull", new Action( instance.TestUnpackFrom_UnpackerIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleObject_Fail", new Action( instance.TestUnpackSingleObject_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleObject_HasExtra_NotAMatter", new Action( instance.TestUnpackSingleObject_HasExtra_NotAMatter ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleObject_Null_Fail", new Action( instance.TestUnpackSingleObject_Null_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleObject_Success", new Action( instance.TestUnpackSingleObject_Success ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo_CollectionIsNull", new Action( instance.TestUnpackTo_CollectionIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo_IsNotCollectionType", new Action( instance.TestUnpackTo_IsNotCollectionType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo_StreamContainsNull", new Action( instance.TestUnpackTo_StreamContainsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackTo_UnpackerIsNull", new Action( instance.TestUnpackTo_UnpackerIsNull ) ) );
		}
	} 

	internal static class MessagePackStringTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessagePackStringTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessagePackStringTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode_Binary", new Action( instance.TestGetHashCode_Binary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode_EmptyBinary", new Action( instance.TestGetHashCode_EmptyBinary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode_EmptyString", new Action( instance.TestGetHashCode_EmptyString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode_String", new Action( instance.TestGetHashCode_String ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetHashCode_StringifiableBinary", new Action( instance.TestGetHashCode_StringifiableBinary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_Binary", new Action( instance.TestToString_Binary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_EmptyBinary", new Action( instance.TestToString_EmptyBinary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_EmptyString", new Action( instance.TestToString_EmptyString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_String", new Action( instance.TestToString_String ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestToString_StringifiableBinary", new Action( instance.TestToString_StringifiableBinary ) ) );
		}
	} 

	internal static class MessageUnpackableTestInitializer
	{
		public static object CreateInstance()
		{
			return new MessageUnpackableTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( MessageUnpackableTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestImage", new Action( instance.TestImage ) ) );
		}
	} 

	internal static class PackerFactoryTestInitializer
	{
		public static object CreateInstance()
		{
			return new PackerFactoryTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( PackerFactoryTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_DefaultOptions", new Action( instance.TestCreate_DefaultOptions ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_NullStream_ArgumentNullException", new Action( instance.TestCreate_NullStream_ArgumentNullException ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_OwnsStreamIsFalse_StreamIsNotClosedWhenPackerIsDisposed", new Action( instance.TestCreate_OwnsStreamIsFalse_StreamIsNotClosedWhenPackerIsDisposed ) ) );
		}
	} 

	internal static class PackUnpackTestInitializer
	{
		public static object CreateInstance()
		{
			return new PackUnpackTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( PackUnpackTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArray", new Action( instance.TestArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestArray_Splitted", new Action( instance.TestArray_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBoolean", new Action( instance.TestBoolean ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByte", new Action( instance.TestByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestByte_Splitted", new Action( instance.TestByte_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBytes", new Action( instance.TestBytes ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestBytes_Splitted", new Action( instance.TestBytes_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestChars", new Action( instance.TestChars ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestChars_Splitted", new Action( instance.TestChars_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary", new Action( instance.TestDictionary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDictionary_Splitted", new Action( instance.TestDictionary_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDouble", new Action( instance.TestDouble ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDouble_Splitted", new Action( instance.TestDouble_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExts", new Action( instance.TestExts ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExts_Splitted", new Action( instance.TestExts_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestHeteroArray", new Action( instance.TestHeteroArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt16", new Action( instance.TestInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt16_Splitted", new Action( instance.TestInt16_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32", new Action( instance.TestInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt32_Splitted", new Action( instance.TestInt32_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64", new Action( instance.TestInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestInt64_Splitted", new Action( instance.TestInt64_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMultipleObjectInStream", new Action( instance.TestMultipleObjectInStream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNestedArray", new Action( instance.TestNestedArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNestedMap", new Action( instance.TestNestedMap ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestNil", new Action( instance.TestNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSByte", new Action( instance.TestSByte ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSByte_Splitted", new Action( instance.TestSByte_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSingle", new Action( instance.TestSingle ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSingle_Splitted", new Action( instance.TestSingle_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestStringShort", new Action( instance.TestStringShort ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt16", new Action( instance.TestUInt16 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt16_Splitted", new Action( instance.TestUInt16_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt32", new Action( instance.TestUInt32 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt32_Splitted", new Action( instance.TestUInt32_Splitted ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt64", new Action( instance.TestUInt64 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUInt64_Splitted", new Action( instance.TestUInt64_Splitted ) ) );
		}
	} 

	internal static class ReflectionBasedNilImplicationTestInitializer
	{
		public static object CreateInstance()
		{
			return new ReflectionBasedNilImplicationTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( ReflectionBasedNilImplicationTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_NullableValueType_AllOk", new Action( instance.TestCreation_NullableValueType_AllOk ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_ReadOnlyCollectionProperty_OnlyNullIsInvalid", new Action( instance.TestCreation_ReadOnlyCollectionProperty_OnlyNullIsInvalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_ReferenceType_AllOk", new Action( instance.TestCreation_ReferenceType_AllOk ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreation_ValueType_OnlyNullIsInvalid", new Action( instance.TestCreation_ValueType_OnlyNullIsInvalid ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestElelementMissingInTheFirstPlace_Map_MissingMembersAreSkipped", new Action( instance.TestElelementMissingInTheFirstPlace_Map_MissingMembersAreSkipped ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestElelementTooManyInTheFirstPlace_Map_ExtrasAreIgnored", new Action( instance.TestElelementTooManyInTheFirstPlace_Map_ExtrasAreIgnored ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_NullableValueType_ProhibitWillFail_Array", new Action( instance.TestPack_NullableValueType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_NullableValueType_ProhibitWillFail_Map", new Action( instance.TestPack_NullableValueType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Array", new Action( instance.TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Map", new Action( instance.TestPack_ReadOnlyCollectionProperty_Prohibit_Fail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReferenceType_ProhibitWillFail_Array", new Action( instance.TestPack_ReferenceType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestPack_ReferenceType_ProhibitWillFail_Map", new Action( instance.TestPack_ReferenceType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Array", new Action( instance.TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Map", new Action( instance.TestUnpack_NullableValueType_MemberDefaultWillBePreserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_NullWillBeNull_Array", new Action( instance.TestUnpack_NullableValueType_NullWillBeNull_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_NullWillBeNull_Map", new Action( instance.TestUnpack_NullableValueType_NullWillBeNull_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_ProhibitWillFail_Array", new Action( instance.TestUnpack_NullableValueType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_NullableValueType_ProhibitWillFail_Map", new Action( instance.TestUnpack_NullableValueType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Array", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Map", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_MemberDefault_Preserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Array", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Map", new Action( instance.TestUnpack_ReadOnlyCollectionProperty_Prohibit_Fail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Array", new Action( instance.TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Map", new Action( instance.TestUnpack_ReferenceType_MemberDefaultWillBePreserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_NullWillBeNull_Array", new Action( instance.TestUnpack_ReferenceType_NullWillBeNull_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_NullWillBeNull_Map", new Action( instance.TestUnpack_ReferenceType_NullWillBeNull_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_ProhibitWillFail_Array", new Action( instance.TestUnpack_ReferenceType_ProhibitWillFail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ReferenceType_ProhibitWillFail_Map", new Action( instance.TestUnpack_ReferenceType_ProhibitWillFail_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_MemberDefault_Preserved_Array", new Action( instance.TestUnpack_ValueType_MemberDefault_Preserved_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_MemberDefault_Preserved_Map", new Action( instance.TestUnpack_ValueType_MemberDefault_Preserved_Map ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_Prohibit_Fail_Array", new Action( instance.TestUnpack_ValueType_Prohibit_Fail_Array ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpack_ValueType_Prohibit_Fail_Map", new Action( instance.TestUnpack_ValueType_Prohibit_Fail_Map ) ) );
		}
	} 

	internal static class RegressionTestsInitializer
	{
		public static object CreateInstance()
		{
			return new RegressionTests();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( RegressionTests )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "Issue143", new Action( instance.Issue143 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDateTimeTypeError_SerializationException", new Action( instance.TestDateTimeTypeError_SerializationException ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue124_AotForComplexValueType", new Action( instance.TestIssue124_AotForComplexValueType ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue211_DictionaryOfT", new Action( instance.TestIssue211_DictionaryOfT ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue211_ListOfT", new Action( instance.TestIssue211_ListOfT ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue211_QueueOfT", new Action( instance.TestIssue211_QueueOfT ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue211_StackOfT", new Action( instance.TestIssue211_StackOfT ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue70", new Action( instance.TestIssue70 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue73", new Action( instance.TestIssue73 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue92_EmptyAsCollection", new Action( instance.TestIssue92_EmptyAsCollection ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue92_EmptyAsMpo", new Action( instance.TestIssue92_EmptyAsMpo ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue99_HoGyuLee_AotForEnumKeyDictionary", new Action( instance.TestIssue99_HoGyuLee_AotForEnumKeyDictionary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMessagePackObject_Binary_PackToMessage_ToBianry", new Action( instance.TestMessagePackObject_Binary_PackToMessage_ToBianry ) ) );
		}
	} 

	internal static class SerializationContextTestInitializer
	{
		public static object CreateInstance()
		{
			return new SerializationContextTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( SerializationContextTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestConfigureClassic_DefaultIsReplaced", new Action( instance.TestConfigureClassic_DefaultIsReplaced ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreateClassicContext_Version0_5_Compatible", new Action( instance.TestCreateClassicContext_Version0_5_Compatible ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefault_SafeAndEasySettings", new Action( instance.TestDefault_SafeAndEasySettings ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Default_Check", new Action( instance.TestDefaultCollectionTypes_Default_Check ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_AbstractType_Fail", new Action( instance.TestDefaultCollectionTypes_Register_AbstractType_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_ArityIsTooFew_Fail", new Action( instance.TestDefaultCollectionTypes_Register_ArityIsTooFew_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_ArityIsTooMany_Fail", new Action( instance.TestDefaultCollectionTypes_Register_ArityIsTooMany_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_CloseOpen_Fail", new Action( instance.TestDefaultCollectionTypes_Register_CloseOpen_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_Collection_Closed_Ok", new Action( instance.TestDefaultCollectionTypes_Register_Collection_Closed_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_Collection_New_Ok", new Action( instance.TestDefaultCollectionTypes_Register_Collection_New_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_Collection_Overwrite_Ok", new Action( instance.TestDefaultCollectionTypes_Register_Collection_Overwrite_Ok ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_Incompatible_Fail", new Action( instance.TestDefaultCollectionTypes_Register_Incompatible_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_NonCollection_Fail", new Action( instance.TestDefaultCollectionTypes_Register_NonCollection_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_NonGenericGenericMpoOk", new Action( instance.TestDefaultCollectionTypes_Register_NonGenericGenericMpoOk ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultCollectionTypes_Register_OpenClose_Fail", new Action( instance.TestDefaultCollectionTypes_Register_OpenClose_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestDefaultTableCapacity", new Action( instance.TestDefaultTableCapacity ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetSerializer", new Action( instance.TestGetSerializer ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetSerializer_Null", new Action( instance.TestGetSerializer_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestGetSerializer_Type", new Action( instance.TestGetSerializer_Type ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue24", new Action( instance.TestIssue24 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue27_Collection", new Action( instance.TestIssue27_Collection ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue27_Dictionary", new Action( instance.TestIssue27_Dictionary ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestIssue27_List", new Action( instance.TestIssue27_List ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_AddRemove_Removed", new Action( instance.TestResolveSerializer_AddRemove_Removed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_BuiltIns_NotRaised", new Action( instance.TestResolveSerializer_BuiltIns_NotRaised ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_NotRegistered_Raised", new Action( instance.TestResolveSerializer_NotRegistered_Raised ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_Registered_NotRaised", new Action( instance.TestResolveSerializer_Registered_NotRaised ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_SetFound_CanCustomizeSerializer", new Action( instance.TestResolveSerializer_SetFound_CanCustomizeSerializer ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_SetNull_Generated", new Action( instance.TestResolveSerializer_SetNull_Generated ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestResolveSerializer_WrongSerializer_Fail", new Action( instance.TestResolveSerializer_WrongSerializer_Fail ) ) );
		}
	} 

	internal static class StreamPackerTestInitializer
	{
		public static object CreateInstance()
		{
			return new StreamPackerTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( StreamPackerTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_OwnsStreamIsFalse_StreamIsNotClosed", new Action( instance.TestCreate_OwnsStreamIsFalse_StreamIsNotClosed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_OwnsStreamIsTrue_StreamIsClosed", new Action( instance.TestCreate_OwnsStreamIsTrue_StreamIsClosed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_StreamIsNull", new Action( instance.TestCreate_StreamIsNull ) ) );
		}
	} 

	internal static class StructWithDataContractTestInitializer
	{
		public static object CreateInstance()
		{
			return new StructWithDataContractTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( StructWithDataContractTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "ShouldDeserializeStructsWithDataContracts", new Action( instance.ShouldDeserializeStructsWithDataContracts ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "ShouldSerializeStructsWithDataContracts", new Action( instance.ShouldSerializeStructsWithDataContracts ) ) );
		}
	} 

	internal static class UnpackerFactoryTestInitializer
	{
		public static object CreateInstance()
		{
			return new UnpackerFactoryTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( UnpackerFactoryTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_ArrayIsEmpty", new Action( instance.TestCreate_ByteArray_ArrayIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_ArrayIsNull", new Action( instance.TestCreate_ByteArray_ArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_EntireArray", new Action( instance.TestCreate_ByteArray_EntireArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_ArrayIsNull", new Action( instance.TestCreate_ByteArray_Int32_ArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_CollectionValidationLevel", new Action( instance.TestCreate_ByteArray_Int32_CollectionValidationLevel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_DefaultValidationLevel", new Action( instance.TestCreate_ByteArray_Int32_DefaultValidationLevel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_Empty", new Action( instance.TestCreate_ByteArray_Int32_Empty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_NegativeOffset", new Action( instance.TestCreate_ByteArray_Int32_NegativeOffset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_NoneValidationLevel", new Action( instance.TestCreate_ByteArray_Int32_NoneValidationLevel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_Offset", new Action( instance.TestCreate_ByteArray_Int32_Offset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_ByteArray_Int32_TooLargeOffset", new Action( instance.TestCreate_ByteArray_Int32_TooLargeOffset ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_OwnsStreamisFalse_NotDisposeStream", new Action( instance.TestCreate_OwnsStreamisFalse_NotDisposeStream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_Boolean_StreamIsNull", new Action( instance.TestCreate_Stream_Boolean_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_CollectionValidationLevel", new Action( instance.TestCreate_Stream_CollectionValidationLevel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_DefaultValidationLevel", new Action( instance.TestCreate_Stream_DefaultValidationLevel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_NoneValidationLevel", new Action( instance.TestCreate_Stream_NoneValidationLevel ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_PackerUnpackerStreamOptions_StreamIsNull", new Action( instance.TestCreate_Stream_PackerUnpackerStreamOptions_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_PackerUnpackerStreamOptions_UnpackerOptions_StreamIsNull", new Action( instance.TestCreate_Stream_PackerUnpackerStreamOptions_UnpackerOptions_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_Stream_StreamIsNull", new Action( instance.TestCreate_Stream_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_StreamOptionIsNull", new Action( instance.TestCreate_StreamOptionIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestCreate_WithBuffering", new Action( instance.TestCreate_WithBuffering ) ) );
		}
	} 

	internal static class UnpackingTest_ExtInitializer
	{
		public static object CreateInstance()
		{
			return new UnpackingTest_Ext();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( UnpackingTest_Ext )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs0JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs17TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs1TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs255TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs256TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext16_AndBinaryLengthIs65535TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs0JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs17TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs1TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs255TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs256TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65535TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext32_AndBinaryLengthIs65536TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs0JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs17TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs1TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_Ext8_AndBinaryLengthIs255TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt1_AndBinaryLengthIs1TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt16_AndBinaryLengthIs16TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt2_AndBinaryLengthIs2TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt4_AndBinaryLengthIs4TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8HasExtra_NoProblem_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8JustLength_Success_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8JustLength_Success_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8TooShort_Fail_ByteArray", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8TooShort_Fail_Stream", new Action( instance.TestUnpackMessagePackExtendedTypeObject_FixExt8_AndBinaryLengthIs8TooShort_Fail_Stream ) ) );
		}
	} 

	internal static class UnpackingTest_MiscInitializer
	{
		public static object CreateInstance()
		{
			return new UnpackingTest_Misc();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( UnpackingTest_Misc )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_0_Begin_Head", new Action( instance.TestSeekableByteStream_Seek_0_Begin_Head ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_0_Current_DoesNotMove", new Action( instance.TestSeekableByteStream_Seek_0_Current_DoesNotMove ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_0_End_Tail", new Action( instance.TestSeekableByteStream_Seek_0_End_Tail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_1_Begin_HeadPlus1", new Action( instance.TestSeekableByteStream_Seek_1_Begin_HeadPlus1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_1_Current_Plus1", new Action( instance.TestSeekableByteStream_Seek_1_Current_Plus1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_1_End_Fail", new Action( instance.TestSeekableByteStream_Seek_1_End_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_Minus1_Begin_Fail", new Action( instance.TestSeekableByteStream_Seek_Minus1_Begin_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_Minus1_Current_Minus1", new Action( instance.TestSeekableByteStream_Seek_Minus1_Current_Minus1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_Seek_Minus1_End_TailMinus1", new Action( instance.TestSeekableByteStream_Seek_Minus1_End_TailMinus1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_setPosition_0_Head", new Action( instance.TestSeekableByteStream_setPosition_0_Head ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_setPosition_1_HeadPlus1", new Action( instance.TestSeekableByteStream_setPosition_1_HeadPlus1 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestSeekableByteStream_setPosition_Minus1_Fail", new Action( instance.TestSeekableByteStream_setPosition_Minus1_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackArray_ArrayLengthIsGreaterThanInt32MaxValue", new Action( instance.TestUnpackArray_ArrayLengthIsGreaterThanInt32MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackArray_Eof", new Action( instance.TestUnpackArray_Eof ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackArrayLength_ArrayLengthIsGreaterThanInt32MaxValue", new Action( instance.TestUnpackArrayLength_ArrayLengthIsGreaterThanInt32MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinary_BinaryLengthIsGreaterThanInt32MaxValue", new Action( instance.TestUnpackBinary_BinaryLengthIsGreaterThanInt32MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinary_EofInBody", new Action( instance.TestUnpackBinary_EofInBody ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinary_EofInHeader", new Action( instance.TestUnpackBinary_EofInHeader ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinary_Stream_ReadOnlyStream", new Action( instance.TestUnpackBinary_Stream_ReadOnlyStream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinaryResultStreamIsNotWriteable_CanWrite_False", new Action( instance.TestUnpackBinaryResultStreamIsNotWriteable_CanWrite_False ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinaryResultStreamIsNotWriteable_Flush_Nop", new Action( instance.TestUnpackBinaryResultStreamIsNotWriteable_Flush_Nop ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinaryResultStreamIsNotWriteable_SetLength_Fail", new Action( instance.TestUnpackBinaryResultStreamIsNotWriteable_SetLength_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinaryResultStreamIsNotWriteable_Write_Fail", new Action( instance.TestUnpackBinaryResultStreamIsNotWriteable_Write_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBinaryResultStreamIsNotWriteable_WriteByte_Fail", new Action( instance.TestUnpackBinaryResultStreamIsNotWriteable_WriteByte_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Empty", new Action( instance.TestUnpackByteStream_Empty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_EofInBody_CanFeed", new Action( instance.TestUnpackByteStream_EofInBody_CanFeed ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_EofInHeader", new Action( instance.TestUnpackByteStream_EofInHeader ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Nil_AsEmpty", new Action( instance.TestUnpackByteStream_Nil_AsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_NotRaw", new Action( instance.TestUnpackByteStream_NotRaw ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_0x10000Byte_AsIsAndBounded", new Action( instance.TestUnpackByteStream_Stream_0x10000Byte_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_0x100Byte_AsIsAndBounded", new Action( instance.TestUnpackByteStream_Stream_0x100Byte_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_1Byte_AsIsAndBounded", new Action( instance.TestUnpackByteStream_Stream_1Byte_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_Empty_AsIsAndBounded", new Action( instance.TestUnpackByteStream_Stream_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_LengthIsGreaterThanInt32MaxValue_CanReadToEnd", new Action( instance.TestUnpackByteStream_Stream_LengthIsGreaterThanInt32MaxValue_CanReadToEnd ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_Null", new Action( instance.TestUnpackByteStream_Stream_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_SeekableStream_CanSeekIsFalse", new Action( instance.TestUnpackByteStream_Stream_SeekableStream_CanSeekIsFalse ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteStream_Stream_SeekableStream_CanSeekIsTrue", new Action( instance.TestUnpackByteStream_Stream_SeekableStream_CanSeekIsTrue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_1ByteNonUtf8String_ExceptionInReaderOperation", new Action( instance.TestUnpackCharStream_Stream_1ByteNonUtf8String_ExceptionInReaderOperation ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_1Char_AsIsAndBounded", new Action( instance.TestUnpackCharStream_Stream_1Char_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Empty_AsIsAndBounded", new Action( instance.TestUnpackCharStream_Stream_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Encoding_1Byte_AsIsAndBounded", new Action( instance.TestUnpackCharStream_Stream_Encoding_1Byte_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Encoding_1ByteNonSpecifiedString_ExceptionInReaderOperation", new Action( instance.TestUnpackCharStream_Stream_Encoding_1ByteNonSpecifiedString_ExceptionInReaderOperation ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Encoding_Empty_AsIsAndBounded", new Action( instance.TestUnpackCharStream_Stream_Encoding_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Encoding_EncodingIsNull", new Action( instance.TestUnpackCharStream_Stream_Encoding_EncodingIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Encoding_StreamIsNull", new Action( instance.TestUnpackCharStream_Stream_Encoding_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackCharStream_Stream_Null", new Action( instance.TestUnpackCharStream_Stream_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDictionary_DictionaryCountIsGreaterThanInt32MaxValue", new Action( instance.TestUnpackDictionary_DictionaryCountIsGreaterThanInt32MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDictionary_KeyDuplicated", new Action( instance.TestUnpackDictionary_KeyDuplicated ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDictionaryCount_DictionaryCountIsGreaterThanInt32MaxValue", new Action( instance.TestUnpackDictionaryCount_DictionaryCountIsGreaterThanInt32MaxValue ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt32_Eof", new Action( instance.TestUnpackInt32_Eof ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt32_NotNumeric", new Action( instance.TestUnpackInt32_NotNumeric ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_ByteArrayIsEmpty", new Action( instance.TestUnpackObject_ByteArray_ByteArrayIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_ByteArrayIsNull", new Action( instance.TestUnpackObject_ByteArray_ByteArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_Int32_ByteArrayIsEmpty", new Action( instance.TestUnpackObject_ByteArray_Int32_ByteArrayIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_Int32_ByteArrayIsNull", new Action( instance.TestUnpackObject_ByteArray_Int32_ByteArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_Int32_OffsetIsNegative", new Action( instance.TestUnpackObject_ByteArray_Int32_OffsetIsNegative ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_Int32_OffsetIsTooBig", new Action( instance.TestUnpackObject_ByteArray_Int32_OffsetIsTooBig ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_Int32_Scalar_AsIs", new Action( instance.TestUnpackObject_ByteArray_Int32_Scalar_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_ByteArray_Scalar_AsIs", new Action( instance.TestUnpackObject_ByteArray_Scalar_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_Array_AsIs", new Action( instance.TestUnpackObject_Stream_Array_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_EmptyArray_AsIs", new Action( instance.TestUnpackObject_Stream_EmptyArray_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_EmptyMap_AsIs", new Action( instance.TestUnpackObject_Stream_EmptyMap_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_EmptyRaw_AsIs", new Action( instance.TestUnpackObject_Stream_EmptyRaw_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_Map_AsIs", new Action( instance.TestUnpackObject_Stream_Map_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_NestedArray_AsIs", new Action( instance.TestUnpackObject_Stream_NestedArray_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_NestedMap_AsIs", new Action( instance.TestUnpackObject_Stream_NestedMap_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_Nil_AsIs", new Action( instance.TestUnpackObject_Stream_Nil_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_Null", new Action( instance.TestUnpackObject_Stream_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_Raw_AsIs", new Action( instance.TestUnpackObject_Stream_Raw_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackObject_Stream_Scalar_AsIs", new Action( instance.TestUnpackObject_Stream_Scalar_AsIs ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_1ByteNonUtf8String_ExceptionInReaderOperation", new Action( instance.TestUnpackString_ByteArray_1ByteNonUtf8String_ExceptionInReaderOperation ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_1Char_AsIsAndBounded", new Action( instance.TestUnpackString_ByteArray_1Char_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_ByteArrayIsEmpty", new Action( instance.TestUnpackString_ByteArray_ByteArrayIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_ByteArrayIsNull", new Action( instance.TestUnpackString_ByteArray_ByteArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Empty_AsIsAndBounded", new Action( instance.TestUnpackString_ByteArray_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Encoding_1Byte_AsIsAndBounded", new Action( instance.TestUnpackString_ByteArray_Encoding_1Byte_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Encoding_1ByteNonSpecifiedString", new Action( instance.TestUnpackString_ByteArray_Encoding_1ByteNonSpecifiedString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Encoding_Empty_AsIsAndBounded", new Action( instance.TestUnpackString_ByteArray_Encoding_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_ByteArrayIsEmpty", new Action( instance.TestUnpackString_ByteArray_Int32_ByteArrayIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_ByteArrayIsNull", new Action( instance.TestUnpackString_ByteArray_Int32_ByteArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_Encoding_ByteArrayIsEmpty", new Action( instance.TestUnpackString_ByteArray_Int32_Encoding_ByteArrayIsEmpty ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_Encoding_ByteArrayIsNull", new Action( instance.TestUnpackString_ByteArray_Int32_Encoding_ByteArrayIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_Encoding_EncodingIsNull", new Action( instance.TestUnpackString_ByteArray_Int32_Encoding_EncodingIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_Encoding_OffsetIsNegative", new Action( instance.TestUnpackString_ByteArray_Int32_Encoding_OffsetIsNegative ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_Encoding_OffsetIsTooBig", new Action( instance.TestUnpackString_ByteArray_Int32_Encoding_OffsetIsTooBig ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_OffsetIsNegative", new Action( instance.TestUnpackString_ByteArray_Int32_OffsetIsNegative ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Int32_OffsetIsTooBig", new Action( instance.TestUnpackString_ByteArray_Int32_OffsetIsTooBig ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_ByteArray_Null", new Action( instance.TestUnpackString_ByteArray_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_1ByteNonUtf8String", new Action( instance.TestUnpackString_Stream_1ByteNonUtf8String ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_1Char_AsIsAndBounded", new Action( instance.TestUnpackString_Stream_1Char_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Empty_AsIsAndBounded", new Action( instance.TestUnpackString_Stream_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Encoding_1Byte_AsIsAndBounded", new Action( instance.TestUnpackString_Stream_Encoding_1Byte_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Encoding_1ByteNonSpecifiedString", new Action( instance.TestUnpackString_Stream_Encoding_1ByteNonSpecifiedString ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Encoding_Empty_AsIsAndBounded", new Action( instance.TestUnpackString_Stream_Encoding_Empty_AsIsAndBounded ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Encoding_EncodingIsNull", new Action( instance.TestUnpackString_Stream_Encoding_EncodingIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Encoding_StreamIsNull", new Action( instance.TestUnpackString_Stream_Encoding_StreamIsNull ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackString_Stream_Null", new Action( instance.TestUnpackString_Stream_Null ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnseekableByteStream_getPosition", new Action( instance.TestUnseekableByteStream_getPosition ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnseekableByteStream_Seek", new Action( instance.TestUnseekableByteStream_Seek ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnseekableByteStream_setPosition", new Action( instance.TestUnseekableByteStream_setPosition ) ) );
		}
	} 

	internal static class UnpackingTest_RawInitializer
	{
		public static object CreateInstance()
		{
			return new UnpackingTest_Raw();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( UnpackingTest_Raw )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_0_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin16_0_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_0_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin16_0_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_0_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin16_0_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_0_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin16_0_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_1_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin16_1_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_1_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin16_1_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_1_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin16_1_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_1_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin16_1_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_1_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin16_1_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_1_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin16_1_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_255_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin16_255_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_255_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin16_255_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_255_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin16_255_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_255_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin16_255_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_255_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin16_255_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_255_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin16_255_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_256_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin16_256_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_256_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin16_256_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_256_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin16_256_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_256_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin16_256_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_256_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin16_256_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_256_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin16_256_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_31_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin16_31_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_31_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin16_31_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_31_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin16_31_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_31_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin16_31_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_31_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin16_31_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_31_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin16_31_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_32_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin16_32_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_32_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin16_32_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_32_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin16_32_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_32_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin16_32_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_32_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin16_32_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin16_32_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin16_32_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_0_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin32_0_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_0_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin32_0_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_0_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin32_0_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_0_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin32_0_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_1_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin32_1_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_1_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin32_1_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_1_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin32_1_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_1_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin32_1_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_1_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin32_1_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_1_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin32_1_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_255_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin32_255_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_255_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin32_255_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_255_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin32_255_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_255_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin32_255_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_255_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin32_255_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_255_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin32_255_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_256_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin32_256_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_256_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin32_256_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_256_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin32_256_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_256_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin32_256_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_256_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin32_256_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_256_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin32_256_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_31_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin32_31_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_31_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin32_31_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_31_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin32_31_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_31_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin32_31_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_31_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin32_31_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_31_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin32_31_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_32_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin32_32_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_32_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin32_32_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_32_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin32_32_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_32_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin32_32_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_32_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin32_32_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin32_32_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin32_32_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_0_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin8_0_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_0_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin8_0_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_0_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin8_0_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_0_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin8_0_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_1_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin8_1_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_1_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin8_1_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_1_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin8_1_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_1_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin8_1_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_1_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin8_1_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_1_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin8_1_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_255_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin8_255_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_255_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin8_255_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_255_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin8_255_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_255_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin8_255_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_255_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin8_255_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_255_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin8_255_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_31_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin8_31_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_31_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin8_31_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_31_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin8_31_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_31_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin8_31_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_31_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin8_31_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_31_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin8_31_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_32_AsString_UnpackString_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackBin8_32_AsString_UnpackString_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_32_AsString_UnpackString_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackBin8_32_AsString_UnpackString_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_32_AsString_UnpackString_JustLength_Success_ByteArray", new Action( instance.TestUnpackBin8_32_AsString_UnpackString_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_32_AsString_UnpackString_JustLength_Success_Stream", new Action( instance.TestUnpackBin8_32_AsString_UnpackString_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_32_AsString_UnpackString_TooShort_Fail_ByteArray", new Action( instance.TestUnpackBin8_32_AsString_UnpackString_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackBin8_32_AsString_UnpackString_TooShort_Fail_Stream", new Action( instance.TestUnpackBin8_32_AsString_UnpackString_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackFixStr_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackFixStr_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackFixStr_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackFixStr_0_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackFixStr_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackFixStr_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackFixStr_0_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_0_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackFixStr_0_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackFixStr_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackFixStr_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackFixStr_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackFixStr_1_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackFixStr_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackFixStr_1_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackFixStr_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackFixStr_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackFixStr_1_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackFixStr_1_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackFixStr_1_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_1_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackFixStr_1_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackFixStr_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackFixStr_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackFixStr_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackFixStr_31_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackFixStr_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackFixStr_31_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackFixStr_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackFixStr_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackFixStr_31_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackFixStr_31_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackFixStr_31_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackFixStr_31_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackFixStr_31_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_0_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_0_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_0_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_0_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_1_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_1_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_1_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_1_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_1_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_1_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_1_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_255_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_255_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_255_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_255_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_255_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_255_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_255_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_255_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_255_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_255_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_255_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_255_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_255_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_256_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_256_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_256_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_256_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_256_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_256_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_256_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_256_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_256_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_256_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_256_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_256_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_256_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_31_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_31_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_31_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_31_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_31_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_31_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_31_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_32_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_32_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_32_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_32_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_32_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_32_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr16_32_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr16_32_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr16_32_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr16_32_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr16_32_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr16_32_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr16_32_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_0_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_0_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_0_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_0_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_1_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_1_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_1_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_1_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_1_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_1_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_1_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_255_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_255_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_255_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_255_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_255_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_255_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_255_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_255_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_255_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_255_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_255_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_255_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_255_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_256_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_256_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_256_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_256_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_256_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_256_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_256_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_256_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_256_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_256_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_256_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_256_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_256_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_31_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_31_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_31_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_31_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_31_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_31_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_31_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_32_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_32_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_32_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_32_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_32_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_32_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr32_32_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr32_32_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr32_32_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr32_32_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr32_32_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr32_32_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr32_32_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_0_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_0_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_0_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_0_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_0_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_0_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_0_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_0_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_0_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_1_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_1_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_1_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_1_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_1_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_1_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_1_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_1_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_1_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_1_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_1_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_1_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_1_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_255_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_255_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_255_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_255_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_255_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_255_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_255_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_255_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_255_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_255_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_255_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_255_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_255_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_31_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_31_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_31_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_31_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_31_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_31_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_31_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_31_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_31_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_31_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_31_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_31_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_31_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_32_AsBinary_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_32_AsBinary_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsBinary_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_32_AsBinary_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsBinary_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_32_AsBinary_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsBinary_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_32_AsBinary_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsBinary_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_32_AsBinary_UnpackBinary_TooShort_Fail_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray", new Action( instance.TestUnpackStr8_32_AsString_UnpackBinary_HasExtra_NoProblem_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsString_UnpackBinary_HasExtra_NoProblem_Stream", new Action( instance.TestUnpackStr8_32_AsString_UnpackBinary_HasExtra_NoProblem_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsString_UnpackBinary_JustLength_Success_ByteArray", new Action( instance.TestUnpackStr8_32_AsString_UnpackBinary_JustLength_Success_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsString_UnpackBinary_JustLength_Success_Stream", new Action( instance.TestUnpackStr8_32_AsString_UnpackBinary_JustLength_Success_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsString_UnpackBinary_TooShort_Fail_ByteArray", new Action( instance.TestUnpackStr8_32_AsString_UnpackBinary_TooShort_Fail_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackStr8_32_AsString_UnpackBinary_TooShort_Fail_Stream", new Action( instance.TestUnpackStr8_32_AsString_UnpackBinary_TooShort_Fail_Stream ) ) );
		}
	} 

	internal static class UnpackingTest_ScalarInitializer
	{
		public static object CreateInstance()
		{
			return new UnpackingTest_Scalar();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( UnpackingTest_Scalar )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteMaxValue_UnpackUInt64_ByteArray", new Action( instance.TestUnpackByteMaxValue_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteMaxValue_UnpackUInt64_Stream", new Action( instance.TestUnpackByteMaxValue_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteMaxValuePlusOne_UnpackUInt64_ByteArray", new Action( instance.TestUnpackByteMaxValuePlusOne_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackByteMaxValuePlusOne_UnpackUInt64_Stream", new Action( instance.TestUnpackByteMaxValuePlusOne_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleEpsilon_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleEpsilon_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleEpsilon_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleEpsilon_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleMaxValue_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleMaxValue_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleMaxValue_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleMaxValue_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleMinValue_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleMinValue_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleMinValue_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleMinValue_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNNegativeMaxValue_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleNaNNegativeMaxValue_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNNegativeMaxValue_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleNaNNegativeMaxValue_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNNegativeMinValue_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleNaNNegativeMinValue_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNNegativeMinValue_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleNaNNegativeMinValue_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNPositiveMaxValue_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleNaNPositiveMaxValue_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNPositiveMaxValue_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleNaNPositiveMaxValue_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNPositiveMinValue_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleNaNPositiveMinValue_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNaNPositiveMinValue_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleNaNPositiveMinValue_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNegativeInfinity_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleNegativeInfinity_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNegativeInfinity_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleNegativeInfinity_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNegativeZero_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoubleNegativeZero_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoubleNegativeZero_UnpackDouble_Stream", new Action( instance.TestUnpackDoubleNegativeZero_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoublePositiveInfinity_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoublePositiveInfinity_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoublePositiveInfinity_UnpackDouble_Stream", new Action( instance.TestUnpackDoublePositiveInfinity_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoublePositiveZero_UnpackDouble_ByteArray", new Action( instance.TestUnpackDoublePositiveZero_UnpackDouble_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackDoublePositiveZero_UnpackDouble_Stream", new Action( instance.TestUnpackDoublePositiveZero_UnpackDouble_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt16MinValue_UnpackInt64_ByteArray", new Action( instance.TestUnpackInt16MinValue_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt16MinValue_UnpackInt64_Stream", new Action( instance.TestUnpackInt16MinValue_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt16MinValueMinusOne_UnpackInt64_ByteArray", new Action( instance.TestUnpackInt16MinValueMinusOne_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt16MinValueMinusOne_UnpackInt64_Stream", new Action( instance.TestUnpackInt16MinValueMinusOne_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt32MinValue_UnpackInt64_ByteArray", new Action( instance.TestUnpackInt32MinValue_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt32MinValue_UnpackInt64_Stream", new Action( instance.TestUnpackInt32MinValue_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt32MinValueMinusOne_UnpackInt64_ByteArray", new Action( instance.TestUnpackInt32MinValueMinusOne_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt32MinValueMinusOne_UnpackInt64_Stream", new Action( instance.TestUnpackInt32MinValueMinusOne_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt64MinValue_UnpackInt64_ByteArray", new Action( instance.TestUnpackInt64MinValue_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackInt64MinValue_UnpackInt64_Stream", new Action( instance.TestUnpackInt64MinValue_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMinusOne_UnpackInt64_ByteArray", new Action( instance.TestUnpackMinusOne_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackMinusOne_UnpackInt64_Stream", new Action( instance.TestUnpackMinusOne_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackNegativeFixNumMinValue_UnpackInt64_ByteArray", new Action( instance.TestUnpackNegativeFixNumMinValue_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackNegativeFixNumMinValue_UnpackInt64_Stream", new Action( instance.TestUnpackNegativeFixNumMinValue_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackNegativeFixNumMinValueMinusOne_UnpackInt64_ByteArray", new Action( instance.TestUnpackNegativeFixNumMinValueMinusOne_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackNegativeFixNumMinValueMinusOne_UnpackInt64_Stream", new Action( instance.TestUnpackNegativeFixNumMinValueMinusOne_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackPlusOne_UnpackUInt64_ByteArray", new Action( instance.TestUnpackPlusOne_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackPlusOne_UnpackUInt64_Stream", new Action( instance.TestUnpackPlusOne_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackPositiveFixNumMaxValue_UnpackUInt64_ByteArray", new Action( instance.TestUnpackPositiveFixNumMaxValue_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackPositiveFixNumMaxValue_UnpackUInt64_Stream", new Action( instance.TestUnpackPositiveFixNumMaxValue_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackPositiveFixNumMaxValuePlusOne_UnpackUInt64_ByteArray", new Action( instance.TestUnpackPositiveFixNumMaxValuePlusOne_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackPositiveFixNumMaxValuePlusOne_UnpackUInt64_Stream", new Action( instance.TestUnpackPositiveFixNumMaxValuePlusOne_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSByteMinValue_UnpackInt64_ByteArray", new Action( instance.TestUnpackSByteMinValue_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSByteMinValue_UnpackInt64_Stream", new Action( instance.TestUnpackSByteMinValue_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSByteMinValueMinusOne_UnpackInt64_ByteArray", new Action( instance.TestUnpackSByteMinValueMinusOne_UnpackInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSByteMinValueMinusOne_UnpackInt64_Stream", new Action( instance.TestUnpackSByteMinValueMinusOne_UnpackInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleEpsilon_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleEpsilon_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleEpsilon_UnpackSingle_Stream", new Action( instance.TestUnpackSingleEpsilon_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleMaxValue_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleMaxValue_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleMaxValue_UnpackSingle_Stream", new Action( instance.TestUnpackSingleMaxValue_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleMinValue_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleMinValue_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleMinValue_UnpackSingle_Stream", new Action( instance.TestUnpackSingleMinValue_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNNegativeMaxValue_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleNaNNegativeMaxValue_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNNegativeMaxValue_UnpackSingle_Stream", new Action( instance.TestUnpackSingleNaNNegativeMaxValue_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNNegativeMinValue_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleNaNNegativeMinValue_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNNegativeMinValue_UnpackSingle_Stream", new Action( instance.TestUnpackSingleNaNNegativeMinValue_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNPositiveMaxValue_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleNaNPositiveMaxValue_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNPositiveMaxValue_UnpackSingle_Stream", new Action( instance.TestUnpackSingleNaNPositiveMaxValue_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNPositiveMinValue_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleNaNPositiveMinValue_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNaNPositiveMinValue_UnpackSingle_Stream", new Action( instance.TestUnpackSingleNaNPositiveMinValue_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNegativeInfinity_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleNegativeInfinity_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNegativeInfinity_UnpackSingle_Stream", new Action( instance.TestUnpackSingleNegativeInfinity_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNegativeZero_UnpackSingle_ByteArray", new Action( instance.TestUnpackSingleNegativeZero_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSingleNegativeZero_UnpackSingle_Stream", new Action( instance.TestUnpackSingleNegativeZero_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSinglePositiveInfinity_UnpackSingle_ByteArray", new Action( instance.TestUnpackSinglePositiveInfinity_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSinglePositiveInfinity_UnpackSingle_Stream", new Action( instance.TestUnpackSinglePositiveInfinity_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSinglePositiveZero_UnpackSingle_ByteArray", new Action( instance.TestUnpackSinglePositiveZero_UnpackSingle_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackSinglePositiveZero_UnpackSingle_Stream", new Action( instance.TestUnpackSinglePositiveZero_UnpackSingle_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt16MaxValue_UnpackUInt64_ByteArray", new Action( instance.TestUnpackUInt16MaxValue_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt16MaxValue_UnpackUInt64_Stream", new Action( instance.TestUnpackUInt16MaxValue_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt16MaxValuePlusOne_UnpackUInt64_ByteArray", new Action( instance.TestUnpackUInt16MaxValuePlusOne_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt16MaxValuePlusOne_UnpackUInt64_Stream", new Action( instance.TestUnpackUInt16MaxValuePlusOne_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt32MaxValue_UnpackUInt64_ByteArray", new Action( instance.TestUnpackUInt32MaxValue_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt32MaxValue_UnpackUInt64_Stream", new Action( instance.TestUnpackUInt32MaxValue_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt32MaxValuePlusOne_UnpackUInt64_ByteArray", new Action( instance.TestUnpackUInt32MaxValuePlusOne_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt32MaxValuePlusOne_UnpackUInt64_Stream", new Action( instance.TestUnpackUInt32MaxValuePlusOne_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt64MaxValue_UnpackUInt64_ByteArray", new Action( instance.TestUnpackUInt64MaxValue_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackUInt64MaxValue_UnpackUInt64_Stream", new Action( instance.TestUnpackUInt64MaxValue_UnpackUInt64_Stream ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackZero_UnpackUInt64_ByteArray", new Action( instance.TestUnpackZero_UnpackUInt64_ByteArray ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestUnpackZero_UnpackUInt64_Stream", new Action( instance.TestUnpackZero_UnpackUInt64_Stream ) ) );
		}
	} 

	internal static class VersioningTestInitializer
	{
		public static object CreateInstance()
		{
			return new VersioningTest();
		}

		public static void InitializeInstance( TestClassInstance testClassInstance, object testFixtureInstance )
		{
			var instance = ( ( VersioningTest )testFixtureInstance );
			testClassInstance.TestMethods.Add( new TestMethod( "Issue199", new Action( instance.Issue199 ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExtraField_NotExtensible_Array_ReflectionBased_Classic_Fail", new Action( instance.TestExtraField_NotExtensible_Array_ReflectionBased_Classic_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExtraField_NotExtensible_Array_ReflectionBased_None_Fail", new Action( instance.TestExtraField_NotExtensible_Array_ReflectionBased_None_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExtraField_NotExtensible_Map_ReflectionBased_Classic_Fail", new Action( instance.TestExtraField_NotExtensible_Map_ReflectionBased_Classic_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestExtraField_NotExtensible_Map_ReflectionBased_None_Fail", new Action( instance.TestExtraField_NotExtensible_Map_ReflectionBased_None_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFieldInvalidType_Array_ReflectionBased_Fail", new Action( instance.TestFieldInvalidType_Array_ReflectionBased_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFieldInvalidType_Map_ReflectionBased_Fail", new Action( instance.TestFieldInvalidType_Map_ReflectionBased_Fail ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestFieldModified_Map_ReflectionBased_ExtraIsStoredAsExtensionData_MissingIsTreatedAsNil", new Action( instance.TestFieldModified_Map_ReflectionBased_ExtraIsStoredAsExtensionData_MissingIsTreatedAsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMissingField_Array_ReflectionBased_MissingIsTreatedAsNil", new Action( instance.TestMissingField_Array_ReflectionBased_MissingIsTreatedAsNil ) ) );
			testClassInstance.TestMethods.Add( new TestMethod( "TestMissingField_Map_ReflectionBased_MissingIsTreatedAsNil", new Action( instance.TestMissingField_Map_ReflectionBased_MissingIsTreatedAsNil ) ) );
		}
	} 

}
#endif // !UNITY_METRO && !UNITY_4_5
