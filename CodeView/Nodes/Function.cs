﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace CodeView.Nodes
{
	public class Function : NotableNode
	{
		public Project Project;
		public int Address;
		public int Length;
		public byte Flags;
		public string Description = string.Empty;

		public override object GetProperties()
		{
			return new FunctionProperties { Function = this };
		}

		public override void Reload()
		{
			Nodes.Clear();

			if (Project.Memory is null)
				return;

			var flagStack = new Stack<byte>();
			var flags = Flags;
			var reads = new List<int>();
			var writes = new List<int>();
			var tableReads = new List<int>();
			var tableWrites = new List<int>();
			var branches = new Stack<int>();
			var branchFlags = new Stack<byte>();
			var old = new List<int>();
			var calls = new List<int>();
			var current = Address;

			var nodes = new SortedDictionary<int, TreeNode>();

			var read = true;

			while (read)
			{
				while(old.Contains(current))
				{
					if (branches.Count == 0)
					{
						read = false;
						break;
					}
					else
					{
						current = branches.Pop();
						flags = branchFlags.Pop();
					}
				}

				if (!read)
					break;

				old.Add(current);

				var instruction = Project.Memory[current];
				var instructionType = string.Empty;
				var address = 0;
				var addressType = string.Empty;
				var text = string.Empty;
				var next = -1;
				var branch = -1;

				switch (instruction)
				{
					case 0x00:
						text = current.ToString("X6") + " Break";
						instructionType = "Break";
						next = current + 1;
						break;

					case 0x02:
						text = current.ToString("X6") + " Coprocessor Enable";
						instructionType = "Break";
						next = current + 1;
						break;

					case 0x03:
						address = (int)Project.Memory[current + 1];
						text = current.ToString("X6") + " OrAccumulatorWithStackRelativeAddress " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Stack";
						next = current + 2;
						break;

					case 0x05:
						address = (int)Project.Memory[current + 1];
						text = current.ToString("X6") + " OrAccumulatorWithDirectAddress " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x06:
						address = (int)Project.Memory[current + 1];
						text = current.ToString("X6") + " ShiftDirectAddressLeft " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x08:
						text = current.ToString("X6") + " PushFlags";
						next = current + 1;
						flagStack.Push(flags);
						break;

					case 0x09:
						int value;
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " OrAccumulatorWithImmediate " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " OrAccumulatorWithImmediate " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0x0a:
						text = current.ToString("X6") + " ShiftAccumulatorLeft";
						next = current + 1;
						break;

					case 0x0b:
						text = current.ToString("X6") + " PushDirectPage";
						next = current + 1;
						break;

					case 0x0d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " OrAccumulatorWithAbsoluteAddress " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x10:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfPositive " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0x15:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " OrAccumulatorWithDirectAddressPlusXIndex " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x16:
						address = (int)Project.Memory[current + 1];
						text = current.ToString("X6") + " ShiftDirectAddressPlusXIndexLeft " + address.ToString("X2");
						instructionType = "Write";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x18:
						text = current.ToString("X6") + " ClearCarryFlag";
						next = current + 1;
						break;

					case 0x1a:
						text = current.ToString("X6") + " IncrementAccumulator";
						next = current + 1;
						break;

					case 0x1e:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " ShiftAbsoluteAddressPlusXIndexLeft " + address.ToString("X4");
						instructionType = "Write";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x20:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8) | (current & 0xff0000);
						text = current.ToString("X6") + " CallAbsoluteAddress " + address.ToString("X6");
						instructionType = "Call";
						addressType = "Absolute";
						next = current + 3;
						branch = address;
						break;

					case 0x21:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " AndAccumulatorWithDirectAddressPlusXIndexPointer " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x22:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8) | (Project.Memory[current + 3] << 16);
						text = current.ToString("X6") + " CallAbsoluteLongAddress " + address.ToString("X6");
						instructionType = "Call";
						addressType = "AbsoluteLong";
						next = current + 4;
						branch = address;
						break;

					case 0x24:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " TestAndAccumulatorWithDirectAddress " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x26:
						address = (int)Project.Memory[current + 1];
						text = current.ToString("X6") + " RotateDirectAddressLeft " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x28:
						text = current.ToString("X6") + " PullFlags";
						next = current + 1;
						if (flagStack.Count != 0)
							flags = flagStack.Pop();
						break;

					case 0x29:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " AndAccumulatorWithImmediate " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " AndAccumulatorWithImmediate " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0x2a:
						text = current.ToString("X6") + " RotateAccumulatorLeft";
						next = current + 1;
						break;

					case 0x2d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " AndAccumulatorWithAbsoluteAddress " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x2f:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " AndAccumulatorWithAbsoluteLongAddress " + address.ToString("X6");
						instructionType = "Read";
						addressType = "AbsoluteLong";
						next = current + 4;
						break;

					case 0x30:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfNegative " + address.ToString("X6");
						instructionType = "Branch";
						addressType = "Relative";
						next = current + 2;
						branch = address;
						break;

					case 0x32:
						address = (int)Project.Memory[current + 1];
						text = current.ToString("X6") + " AndAccumulatorWithDirectAddressPointer " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Pointer";
						next = current + 2;
						break;

					case 0x38:
						text = current.ToString("X6") + " SetCarryFlag";
						next = current + 1;
						break;

					case 0x39:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " AndAccumulatorWithAbsoluteAddressPlusYIndex " + address.ToString("X4");
						instructionType = "Read";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x3a:
						text = current.ToString("X6") + " DecrementAccumulator";
						next = current + 1;
						break;

					case 0x3d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " AndAccumulatorWithAbsoluteAddressPlusXIndex " + address.ToString("X4");
						instructionType = "Read";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x40:
						text = current.ToString("X6") + " ReturnFromInterrupt";
						instructionType = "Return";
						break;

					case 0x46:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " ShiftDirectAddressRight " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x48:
						text = current.ToString("X6") + " PushAccumulator";
						next = current + 1;
						break;

					case 0x49:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " ExclusiveOrAccumulatorWithImmediate " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " ExclusiveOrAccumulatorWithImmediate " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0x4a:
						text = current.ToString("X6") + " ShiftAccumulatorRight";
						next = current + 1;
						break;

					case 0x4b:
						text = current.ToString("X6") + " PushProgramBank";
						next = current + 1;
						break;

					case 0x4c:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8) | current & 0xff0000;
						text = current.ToString("X6") + " JumpToAbsoluteAddress " + address.ToString("X6");
						instructionType = "Jump";
						addressType = "Absolute";
						next = address;
						break;

					case 0x4d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " ExclusiveOrAccumulatorWithAbsoluteAddress " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x50:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfNotOverflow " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0x58:
						text = current.ToString("X6") + " ClearInterruptDisableFlag";
						next = current + 1;
						break;

					case 0x5a:
						text = current.ToString("X6") + " PushYIndex";
						next = current + 1;
						break;

					case 0x5b:
						text = current.ToString("X6") + " CopyAccumulatorToDirectPageRegister";
						next = current + 1;
						break;

					case 0x5c:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8) | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " JumpToAbsoluteLongAddress " + address.ToString("X6");
						instructionType = "Jump";
						addressType = "AbsoluteLong";
						next = address;
						break;

					case 0x60:
						text = current.ToString("X6") + " ReturnToCaller";
						instructionType = "Return";
						break;

					case 0x63:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " AddStackRelativeAddressToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Stack";
						next = current + 2;
						break;

					case 0x64:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " SetDirectAddressToZero " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x65:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " AddDirectAddressToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x66:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " RotateDirectAddressRight " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x67:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " AddDirectAddressLongPointerToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectLongPointer";
						next = current + 2;
						break;

					case 0x68:
						text = current.ToString("X6") + " PullAccumulator";
						next = current + 1;
						break;

					case 0x69:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " AddImmediateToAccumulator " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " AddImmediateToAccumulator " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0x6a:
						text = current.ToString("X6") + " RotateAccumulatorRight";
						next = current + 1;
						break;

					case 0x6b:
						text = current.ToString("X6") + " ReturnToLongCaller";
						instructionType = "Return";
						break;

					case 0x6d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " AddAbsoluteAddressToAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x6e:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " RotateAbsoluteAddressRight " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x70:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfOverflow " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0x74:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " SetDirectAddressPlusXIndexToZero " + address.ToString("X2");
						instructionType = "Write";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x75:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " AddDirectAddressPlusXIndexToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x78:
						text = current.ToString("X6") + " SetInterruptDisableFlag";
						next = current + 1;
						break;

					case 0x7a:
						text = current.ToString("X6") + " PullYIndex";
						next = current + 1;
						break;

					case 0x7c:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " JumpToAbsoluteAddressPlusXIndexPointer " + address.ToString("X4");
						instructionType = "Jump";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x7e:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " RotateAbsoluteAddressPlusXIndexRight " + address.ToString("X4");
						instructionType = "Write";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x7f:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " AddAbsoluteLongAddressPlusXIndexToAccumulator " + address.ToString("X6");
						instructionType = "Read";
						addressType = "AbsoluteLongTable";
						next = current + 4;
						break;

					case 0x80:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " JumpToRelative " + address.ToString("X6");
						instructionType = "Jump";
						addressType = "Relative";
						next = address;
						break;

					case 0x82:
						address = current + 3 + BitConverter.ToInt16(Project.Memory, current + 1);
						text = current.ToString("X6") + " JumpToRelativeLong " + address.ToString("X6");
						instructionType = "Jump";
						addressType = "RelativeLong";
						next = address;
						break;

					case 0x84:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyYIndexToDirectAddress " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x85:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyAccumulatorToDirectAddress " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x86:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyXIndexToDirectAddress " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0x88:
						text = current.ToString("X6") + " DecrementYIndex";
						next = current + 1;
						break;

					case 0x89:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " TestAndAccumulatorWithImmediate " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " TestAndAccumulatorWithImmediate " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0x8a:
						text = current.ToString("X6") + " CopyXIndexToAccumulator";
						next = current + 1;
						break;

					case 0x8b:
						text = current.ToString("X6") + " PushDataBank";
						next = current + 1;
						break;

					case 0x8c:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyYIndexToAbsoluteAddress " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x8d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAccumulatorToAbsoluteAddress " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x8e:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyXIndexToAbsoluteAddress " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x8f:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " CopyAccumulatorToAbsoluteLongAddress " + address.ToString("X6");
						instructionType = "Write";
						addressType = "AbsoluteLong";
						next = current + 4;
						break;

					case 0x90:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfLessThan " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0x92:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyAccumulatorToDirectAddressPointer " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Pointer";
						next = current + 2;
						break;

					case 0x94:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyYIndexToDirectAddressPlusXIndex " + address.ToString("X2");
						instructionType = "Write";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x95:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyAccumulatorToDirectAddressPlusXIndex " + address.ToString("X2");
						instructionType = "Write";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x96:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyXIndexToDirectAddressPlusYIndex " + address.ToString("X2");
						instructionType = "Write";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0x98:
						text = current.ToString("X6") + " CopyYIndexToAccumulator";
						next = current + 1;
						break;

					case 0x99:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAccumulatorToAbsoluteAddressPlusYIndex " + address.ToString("X4");
						instructionType = "Write";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x9a:
						text = current.ToString("X6") + " CopyXIndexToStackPointer";
						next = current + 1;
						break;

					case 0x9b:
						text = current.ToString("X6") + " CopyXIndexToYIndex";
						next = current + 1;
						break;

					case 0x9c:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " SetAbsoluteAddressToZero " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0x9d:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAccumulatorToAbsoluteAddressPlusXIndex " + address.ToString("X4");
						instructionType = "Write";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x9e:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " SetAbsoluteAddressPlusXIndexToZero " + address.ToString("X4");
						instructionType = "Write";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0x9f:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " CopyAccumulatorToAbsoluteLongAddressPlusXIndex " + address.ToString("X6");
						instructionType = "Write";
						addressType = "AbsoluteLongTable";
						next = current + 4;
						break;

					case 0xa0:
						if ((flags & 0x10) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " CopyImmediateToYIndex " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " CopyImmediateToYIndex " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xa2:
						if ((flags & 0x10) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " CopyImmediateToXIndex " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " CopyImmediateToXIndex " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xa4:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddressToYIndex " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xa5:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddressToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xa6:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddressToXIndex " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xa8:
						text = current.ToString("X6") + " CopyAccumulatorToYIndex";
						next = current + 1;
						break;

					case 0xa9:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " CopyImmediateToAccumulator " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " CopyImmediateToAccumulator " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xaa:
						text = current.ToString("X6") + " CopyAccumulatorToXIndex";
						next = current + 1;
						break;

					case 0xab:
						text = current.ToString("X6") + " PullDataBank";
						next = current + 1;
						break;

					case 0xac:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
						text = current.ToString("X6") + " CopyAbsoluteAddressToYIndex " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xad:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAbsoluteAddressToAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xae:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAbsoluteAddressToXIndex " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xaf:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " CopyAbsoluteLongAddressToAccumulator " + address.ToString("X6");
						instructionType = "Read";
						addressType = "AbsoluteLong";
						next = current + 4;
						break;

					case 0xb0:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfGreaterOrEqual " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0xb4:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddressPlusXIndexToYIndex " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0xb5:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddressPlusXIndexToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0xb6:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddresPlusYIndexToXIndex " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0xb7:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " CopyDirectAddressLongPointerPlusYIndexToAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectLongPointerTable";
						next = current + 2;
						break;

					case 0xb9:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAbsoluteAddressPlusYIndexToAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0xbb:
						text = current.ToString("X6") + " CopyYIndexToXIndex";
						next = current + 1;
						break;

					case 0xbd:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CopyAbsoluteAddressPlusXIndexToAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0xbf:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " CopyAbsoluteLongAddressPlusXIndexToAccumulator " + address.ToString("X6");
						instructionType = "Read";
						addressType = "AbsoluteLongTable";
						next = current + 4;
						break;

					case 0xc0:
						if ((flags & 0x10) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " TestSubtractImmediateFromYIndex " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " TestSubtractImmediateFromYIndex " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xc2:
						value = Project.Memory[current + 1];
						text = current.ToString("X6") + " ClearImmediateFlags " + value.ToString("X2");
						next = current + 2;
						flags &= (byte)~value;
						break;

					case 0xc5:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " TestSubtractDirectAddressFromAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xc6:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " DecrementDirectAddress " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xc8:
						text = current.ToString("X6") + " IncrementYIndex";
						next = current + 1;
						break;

					case 0xc9:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " TestSubtractImmediateFromAccumulator " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " TestSubtractImmediateFromAccumulator " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xca:
						text = current.ToString("X6") + " DecrementXIndex";
						next = current + 1;
						break;

					case 0xcc:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " TestSubtractAbsoluteAddressFromYIndex " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xcd:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " TestSubtractAbsoluteAddressFromAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xce:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " DecrementAbsoluteAddress " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xd0:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfNotZero " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0xd1:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " TestSubtractDirectAddressPointerPlusYIndexFromAccumulator  " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectPointerTable";
						next = current + 2;
						break;

					case 0xd4:
						value = Project.Memory[current + 1];
						text = current.ToString("X6") + " PushPointer " + value.ToString("X2");
						next = current + 2;
						break;

					case 0xd5:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " TestSubtractDirectAddressPlusXIndexFromAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0xda:
						text = current.ToString("X6") + " PushXIndex";
						next = current + 1;
						break;

					case 0xe0:
						if ((flags & 0x10) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " TestSubtractImmediateFromXIndex " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " TestSubtractImmediateFromXIndex " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xe2:
						value = Project.Memory[current + 1];
						text = current.ToString("X6") + " SetImmediateFlags " + value.ToString("X2");
						next = current + 2;
						flags |= (byte)value;
						break;

					case 0xe3:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " SubtractStackRelativeAddressFromAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Stack";
						next = current + 2;
						break;

					case 0xe5:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " SubtractDirectAddressFromAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xe6:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " IncrementDirectAddress " + address.ToString("X2");
						instructionType = "Write";
						addressType = "Direct";
						next = current + 2;
						break;

					case 0xe8:
						text = current.ToString("X6") + " IncrementXIndex";
						next = current + 1;
						break;

					case 0xe9:
						if ((flags & 0x20) == 0)
						{
							value = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
							text = current.ToString("X6") + " SubtractImmediateFromAccumulator " + value.ToString("X4");
							next = current + 3;
						}
						else
						{
							value = Project.Memory[current + 1];
							text = current.ToString("X6") + " SubtractImmediateFromAccumulator " + value.ToString("X2");
							next = current + 2;
						}
						break;

					case 0xea:
						text = current.ToString("X6") + " NoOperation";
						next = current + 1;
						break;

					case 0xeb:
						text = current.ToString("X6") + " ExchangeAccumulators";
						next = current + 1;
						break;

					case 0xec:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " TestSubtractAbsoluteAddressFromXIndex " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xed:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " SubtractAbsoluteAddressFromAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xee:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " IncrementAbsoluteAddress " + address.ToString("X4");
						instructionType = "Write";
						addressType = "Absolute";
						next = current + 3;
						break;

					case 0xef:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " SubtractAbsoluteLongAddressFromAccumulator " + address.ToString("X6");
						instructionType = "Read";
						addressType = "AbsoluteLong";
						next = current + 4;
						break;

					case 0xf0:
						address = current + 2 + (sbyte)Project.Memory[current + 1];
						text = current.ToString("X6") + " BranchToRelativeIfZero " + address.ToString("X6");
						instructionType = "Branch";
						next = current + 2;
						branch = address;
						break;

					case 0xf4:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8;
						text = current.ToString("X6") + " PushImmediate " + address.ToString("X4");
						next = current + 3;
						break;

					case 0xf5:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " SubtractDirectAddressPlusXIndexFromAccumulator " + address.ToString("X2");
						instructionType = "Read";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0xf6:
						address = Project.Memory[current + 1];
						text = current.ToString("X6") + " IncrementDirectAddressPlusXIndex " + address.ToString("X2");
						instructionType = "Write";
						addressType = "DirectTable";
						next = current + 2;
						break;

					case 0xf9:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " SubtractAbsoluteAddressPlusYIndexFromAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0xfa:
						text = current.ToString("X6") + " PullXIndex";
						next = current + 1;
						break;

					case 0xfb:
						text = current.ToString("X6") + " ExchangeCarryFlagWithEmulationFlag";
						next = current + 1;
						break;

					case 0xfc:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " CallAbsoluteAddressPlusXIndex " + address.ToString("X4");
						instructionType = "Call";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0xfd:
						address = Project.Memory[current + 1] | (Project.Memory[current + 2] << 8);
						text = current.ToString("X6") + " SubtractAbsoluteAddressPlusXIndexFromAccumulator " + address.ToString("X4");
						instructionType = "Read";
						addressType = "AbsoluteTable";
						next = current + 3;
						break;

					case 0xff:
						address = Project.Memory[current + 1] | Project.Memory[current + 2] << 8 | Project.Memory[current + 3] << 16;
						text = current.ToString("X6") + " SubtractAbsoluteLongAddressPlusXIndexFromAccumulator " + address.ToString("X6");
						instructionType = "Read";
						addressType = "AbsoluteLongTable";
						next = current + 4;
						break;

					default:
						text = current.ToString("X6") + " Unknown Instruction: " + instruction.ToString("X2");
						instructionType = "Unknown";
						break;
				}

				switch (instructionType)
				{
					case "Read":
						switch (addressType)
						{
							case "DirectTable":
							case "DirectPointerTable":
							case "AbsoluteTable":
							case "AbsoluteLongTable":
								tableReads.Add(address);

								if(Project.Tables.TryGetValue(address, out Table table))
									nodes.Add(current, new TablePointer { Text = text + " [" + table.Text + "]", ForeColor = Color.Green, NodeFont = Fonts.Bold, Address = address });
								else
									nodes.Add(current, new TablePointer { Text = text, ForeColor = Color.Green, NodeFont = Fonts.Bold, Address = address });
								break;

							default:
								reads.Add(address);
								if (Project.Variables.TryGetValue(address, out Variable variable))
									nodes.Add(current, new VariablePointer { Text = text + " [" + variable.Text + "]", ForeColor = Color.Green, Address = address });
								else
									nodes.Add(current, new VariablePointer { Text = text, ForeColor = Color.Green, Address = address });
								break;
						}

						current = next;
						break;

					case "Write":
						switch (addressType)
						{
							case "DirectTable":
							case "DirectPointerTable":
							case "AbsoluteTable":
							case "AbsoluteLongTable":
								tableWrites.Add(address);
								if (Project.Tables.TryGetValue(address, out Table table))
									nodes.Add(current, new TablePointer { Text = text + " [" + table.Text + "]", ForeColor = Color.Red, NodeFont = Fonts.Bold, Address = address });
								else
									nodes.Add(current, new TablePointer { Text = text, ForeColor = Color.Red, NodeFont = Fonts.Bold, Address = address });
								break;

							default:
								writes.Add(address);
								if (Project.Variables.TryGetValue(address, out Variable variable))
									nodes.Add(current, new VariablePointer { Text = text + " [" + variable.Text + "]", ForeColor = Color.Red, Address = address });
								else
									nodes.Add(current, new VariablePointer { Text = text, ForeColor = Color.Red, Address = address });
								break;
						}
						current = next;
						break;

					case "Branch":
						if(branch > current)
							nodes.Add(current, new TreeNode { Text = text, ForeColor = Color.Blue });
						else
							nodes.Add(current, new TreeNode { Text = text, ForeColor = Color.Purple });

						if (!branches.Contains(branch) &&
							!old.Contains(branch))
						{
							branches.Push(branch);
							branchFlags.Push(flags);
						}

						current = next;
						break;

					case "Jump":
						switch (addressType)
						{
							case "AbsoluteTable":
								if (Project.Tables.TryGetValue(address, out Table table))
									nodes.Add(current, new TablePointer { Text = text + " [" + table.Text + "]", NodeFont = Fonts.Bold, Address = address });
								else
									nodes.Add(current, new TablePointer { Text = text, NodeFont = Fonts.Bold, Address = address });
								break;

							default:
								if (Project.Functions.TryGetValue(address, out Function function))
									nodes.Add(current, new FunctionPointer { Text = text + " [" + function.Text + "]", NodeFont = Fonts.Bold, Address = address, Flags = flags });
								else
									nodes.Add(current, new FunctionPointer { Text = text, NodeFont = Fonts.Bold, Address = address, Flags = flags });
								break;
						}

						while (read && old.Contains(current))
						{
							if (branches.Count == 0)
								read = false;
							else
							{
								current = branches.Pop();
								flags = branchFlags.Pop();
							}
						}
						break;

					case "Call":
						switch (addressType)
						{
							case "AbsoluteTable":
								if (Project.Tables.TryGetValue(address, out Table table))
									nodes.Add(current, new TablePointer { Text = text + " [" + table.Text + "]", NodeFont = Fonts.Bold, Address = address });
								else
									nodes.Add(current, new TablePointer { Text = text, NodeFont = Fonts.Bold, Address = address });
								break;

							default:
								if (Project.Functions.TryGetValue(address, out Function function))
									nodes.Add(current, new FunctionPointer { Text = text + " [" + function.Text + "]", NodeFont = Fonts.Bold, Address = address, Flags = flags });
								else
									nodes.Add(current, new FunctionPointer { Text = text, NodeFont = Fonts.Bold, Address = address, Flags = flags });
								break;
						}

						current = next;
						break;

					case "Return":
						nodes.Add(current, new TreeNode(text));

						while (read && old.Contains(current))
						{
							if (branches.Count == 0)
								read = false;
							else
							{
								current = branches.Pop();
								flags = branchFlags.Pop();
							}
						}
						break;

					case "Break":
						nodes.Add(current, new TreeNode(text));
						read = false;
						break;

					case "Unknown":
						nodes.Add(current, new TreeNode(text));
						read = false;
						break;

					default:
						nodes.Add(current, new TreeNode(text));
						current = next;
						break;
				}
			}

			Nodes.AddRange(nodes.Select(x => x.Value).ToArray());
		}

		public class FunctionProperties
		{
			public Function Function;

			public string Name
			{
				get => Function.Text;
				set => Function.Text = value;
			}

			public string Description
			{
				get => Function.Description;
				set => Function.Description = value;
			}

			public string Address
			{
				get => Function.Address.ToString("X");
				set
				{
					Function.Project.Functions.Remove(Function.Address);

					Function.Address = int.Parse(value, System.Globalization.NumberStyles.HexNumber);

					Function.Project.Functions.Add(Function.Address, Function);
				}
			}

			public string Flags
			{
				get => Function.Flags.ToString("X");
				set => Function.Flags = byte.Parse(value, System.Globalization.NumberStyles.HexNumber);
			}
		}
	}
}