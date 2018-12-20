// dnlib: See LICENSE.txt for more info

﻿namespace dnlib.DotNet.Emit {
	/// <summary>
	/// A CIL method exception handler
	/// </summary>
	public sealed class ExceptionHandler {
		/// <summary>
		/// First instruction of try block
		/// </summary>
		private Instruction tryStart;

		/// <summary>
		/// One instruction past the end of try block or <c>null</c> if it ends at the end
		/// of the method.
		/// </summary>
		private Instruction tryEnd;

		/// <summary>
		/// Start of filter handler or <c>null</c> if none. The end of filter handler is
		/// always <see cref="HandlerStart"/>.
		/// </summary>
		private Instruction filterStart;

		/// <summary>
		/// First instruction of try handler block
		/// </summary>
		private Instruction handlerStart;

		/// <summary>
		/// One instruction past the end of try handler block or <c>null</c> if it ends at the end
		/// of the method.
		/// </summary>
		private Instruction handlerEnd;

		/// <summary>
		/// The catch type if <see cref="HandlerType"/> is <see cref="ExceptionHandlerType.Catch"/>
		/// </summary>
		private ITypeDefOrRef catchType;

		/// <summary>
		/// Type of exception handler clause
		/// </summary>
		private ExceptionHandlerType handlerType;

		/// <summary>
		/// Encapsulate field
		/// </summary>
		public Instruction TryStart { get => tryStart; set => tryStart = value; }
		/// <summary>
		/// Encapsulate field
		/// </summary>
		public Instruction TryEnd { get => tryEnd; set => tryEnd = value; }
		/// <summary>
		/// Encapsulate field
		/// </summary>
		public Instruction FilterStart { get => filterStart; set => filterStart = value; }
		/// <summary>
		/// Encapsulate field
		/// </summary>
		public Instruction HandlerStart { get => handlerStart; set => handlerStart = value; }
		/// <summary>
		/// Encapsulate field
		/// </summary>
		public Instruction HandlerEnd { get => handlerEnd; set => handlerEnd = value; }
		/// <summary>
		/// Encapsulate field
		/// </summary>
		public ITypeDefOrRef CatchType { get => catchType; set => catchType = value; }
		/// <summary>
		/// Encapsulate field
		/// </summary>
		public ExceptionHandlerType HandlerType { get => handlerType; set => handlerType = value; }

		/// <summary>
		/// Default constructor
		/// </summary>
		public ExceptionHandler() {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="handlerType">Exception clause type</param>
		public ExceptionHandler(ExceptionHandlerType handlerType) => HandlerType = handlerType;
	}
}
