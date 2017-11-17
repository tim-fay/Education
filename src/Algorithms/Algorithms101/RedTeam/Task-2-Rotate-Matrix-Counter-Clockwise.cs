﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Xunit;

namespace Algorithms101.RedTeam
{
    [Trait("Category", "Red Team")]
    public class Task2RotateMatrixCounterClockwise
    {
        private const int MinLength = 2;
        private const int MaxLength = 300;
        private const int MinRotateSteps = 1;
        private const int MaxRotateSteps = 1_000_000_000;

        [Fact]
        public void InputParametersTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => Rotate(new int[MinLength - 1,MinLength], MinRotateSteps));
            Assert.Throws<ArgumentOutOfRangeException>(() => Rotate(new int[MinLength,MinLength - 1], MinRotateSteps));
            Assert.Throws<ArgumentOutOfRangeException>(() => Rotate(new int[MaxLength + 1,MaxLength], MinRotateSteps));
            Assert.Throws<ArgumentOutOfRangeException>(() => Rotate(new int[MaxLength, MaxLength + 1], MinRotateSteps));
            Assert.Throws<ArgumentOutOfRangeException>(() => Rotate(new int[MinLength, MinLength], MinRotateSteps - 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => Rotate(new int[MinLength, MinLength], MaxRotateSteps + 1));
        }

        public static IEnumerable<object[]> TestInputArray1() => new TheoryData<int[,], int, int[,]>
        {
            {
                new[,]
                {
                    { 1, 1 },
                    { 1, 1 }
                },
                3,
                new[,]
                {
                    { 1, 1 },
                    { 1, 1 }
                }
            }
        };

        public static IEnumerable<object[]> TestInputArray2() => new TheoryData<int[,], int, int[,]>
        {
            {
                new[,]
                {
                    { 1, 2, 3, 4 },
                    { 5, 6, 7, 8 },
                    { 9, 10, 11, 12 },
                    { 13, 14, 15, 16 }
                },
                3,
                new[,]
                {
                    {2, 3, 4, 8},
                    {1, 7, 11, 12},
                    {5, 6, 10, 16},
                    {9, 13, 14, 15}
                }
            }
        };

        [Theory]
        [MemberData(nameof(TestInputArray1))]
        [MemberData(nameof(TestInputArray2))]
        public void RegularInputTest(int[,] input, int rotate, int[,] expected)
        {
            Assert.Equal(expected, Rotate(input, rotate));
        }

        private int[,] Rotate(int[,] input, int rotate)
        {
            CheckInput(input, rotate);

            return RotateSingle(0, 0, input.GetLength(0), input.GetLength(1), input, rotate);
        }

        private int[,] RotateSingle(int x, int y, int rows, int columns, int[,] input, int rotate)
        {
            int totalItemsCountToRotate = (rows + columns - 2) * 2;
            int rotationLength = rotate % totalItemsCountToRotate;

            if (rotationLength == 0)
            {
                return input;
            }
            
            var rotateDirection = RotateDirection.Down;

            for (int rotationStep = 0; rotationStep < rotationLength; rotationStep++)
            {
                var stepsToComplete = rotationLength;
                int row = x, column = y;
                int tempValue;

                while (stepsToComplete > 0)
                {
                    switch (rotateDirection)
                    {
                        case RotateDirection.Down:
                            if (stepsToComplete <= rows)
                            {
                                tempValue = input[row + stepsToComplete, column];
                                input[row + stepsToComplete, column] = input[row, column];

                            }
                            else
                            {
                                stepsToComplete = stepsToComplete - rows;
                                rotateDirection = RotateDirection.Right;
                            }
                            break;
                        case RotateDirection.Right:
                            break;
                        case RotateDirection.Up:
                            break;
                        case RotateDirection.Left:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                    stepsToComplete--;
                }
            }

            return input;
        }

        private static void CheckInput(int[,] input, int rotate)
        {
            if (input.Rank != 2
                || Math.Min(input.GetLength(0), input.GetLength(1)) < MinLength
                || Math.Min(input.GetLength(0), input.GetLength(1)) % 2 != 0
                || input.GetLength(0) > MaxLength
                || input.GetLength(1) > MaxLength)
            {
                throw new ArgumentOutOfRangeException(nameof(input));
            }

            if (rotate < MinRotateSteps || rotate > MaxRotateSteps)
            {
                throw new ArgumentOutOfRangeException(nameof(rotate));
            }
        }

        private enum RotateDirection
        {
            Down,
            Right,
            Up,
            Left
        }

        private struct TwoDimensionalArrayOuterFrameSpan<T>
        {
            private readonly T[,] _array;
            private readonly int _row;
            private readonly int _column;
            private readonly int _rowsCount;
            private readonly int _columnsCount;
            private readonly int _leftItemsCount;
            private readonly int _leftBottomItemsCount;
            private readonly int _leftBottomRightItemsCount;

            public TwoDimensionalArrayOuterFrameSpan(T[,] array, int row, int column, int rowsCount, int columnsCount)
            {
                _array = array ?? throw new ArgumentNullException(nameof(array));
                _row = row;
                _column = column;
                _rowsCount = rowsCount;
                _columnsCount = columnsCount;

                _leftItemsCount = _rowsCount - 1;
                _leftBottomItemsCount = _leftItemsCount + _columnsCount - 1;
                _leftBottomRightItemsCount = _leftBottomItemsCount + _rowsCount - 1;
            }

            /// <summary>
            /// Returns a reference 
            /// </summary>
            /// <param name="index">A zero based index of the array.</param>
            /// <returns></returns>
            public ref T this[int index]
            {
                get
                {
                    if (index < _leftItemsCount)
                    {
                        return ref GetLeftSideValue(index);
                    }

                    int indexRemainder = index - _leftItemsCount;
                    if (index < _leftBottomItemsCount)
                    {
                        return ref GetBottomSideValue(indexRemainder);
                    }

                    indexRemainder = index - _leftBottomItemsCount;
                    if (index < _leftBottomRightItemsCount)
                    {
                        return ref GetRightSideValue(indexRemainder);
                    }

                    indexRemainder = index - _leftBottomRightItemsCount;
                    if (index < _leftBottomRightItemsCount)
                    {
                        return ref GetTopSideValue(indexRemainder);
                    }

                    throw new InvalidOperationException();
                }
            }

            public int Length => _rowsCount + _columnsCount - 2;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref T GetLeftSideValue(int index)
            {
                return ref _array[index + _row, _column];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref T GetBottomSideValue(int index)
            {
                return ref _array[_row + _rowsCount - 1, _column + index];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref T GetRightSideValue(int index)
            {
                return ref _array[_row + _rowsCount - 1 - index, _column + _columnsCount - 1];
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private ref T GetTopSideValue(int index)
            {
                return ref _array[_row, _column + _columnsCount - 1 - index];
            }
        }
    }
}