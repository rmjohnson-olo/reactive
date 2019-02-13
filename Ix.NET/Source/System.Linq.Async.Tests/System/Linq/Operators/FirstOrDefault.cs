﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information. 

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Tests
{
    public class FirstOrDefault : AsyncEnumerableTests
    {
        [Fact]
        public async Task FirstOrDefault_Null()
        {
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default).AsTask());

            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default, CancellationToken.None).AsTask());

            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default, x => true).AsTask());
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync(Return42, default(Func<int, bool>)).AsTask());

            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default, x => true, CancellationToken.None).AsTask());
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync(Return42, default(Func<int, bool>), CancellationToken.None).AsTask());

            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default, x => new ValueTask<bool>(true)).AsTask());
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync(Return42, default(Func<int, ValueTask<bool>>)).AsTask());

            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default, x => new ValueTask<bool>(true), CancellationToken.None).AsTask());
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync(Return42, default(Func<int, ValueTask<bool>>), CancellationToken.None).AsTask());

#if !NO_DEEP_CANCELLATION
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync<int>(default, (x, ct) => new ValueTask<bool>(true), CancellationToken.None).AsTask());
            await Assert.ThrowsAsync<ArgumentNullException>(() => AsyncEnumerable.FirstOrDefaultAsync(Return42, default(Func<int, CancellationToken, ValueTask<bool>>), CancellationToken.None).AsTask());
#endif
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Empty()
        {
            var res = AsyncEnumerable.Empty<int>().FirstOrDefaultAsync();
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Empty_Enumerable()
        {
            var res = new int[0].Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync();
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Empty_IList()
        {
            var res = new int[0].ToAsyncEnumerable().FirstOrDefaultAsync();
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Throw()
        {
            var ex = new Exception("Bang!");
            var res = Throw<int>(ex).FirstOrDefaultAsync();
            await AssertThrowsAsync(res, ex);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Single_IList()
        {
            var res = Return42.FirstOrDefaultAsync();
            Assert.Equal(42, await res);
        }


        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Single()
        {
            var res = new[] { 42 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync();
            Assert.Equal(42, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Many_IList()
        {
            var res = new[] { 42, 43, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync();
            Assert.Equal(42, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_NoParam_Many()
        {
            var res = new[] { 42, 43, 44 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync();
            Assert.Equal(42, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Empty()
        {
            var res = AsyncEnumerable.Empty<int>().FirstOrDefaultAsync(x => true);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Throw()
        {
            var ex = new Exception("Bang!");
            var res = Throw<int>(ex).FirstOrDefaultAsync(x => true);
            await AssertThrowsAsync(res, ex);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Single_None()
        {
            var res = Return42.FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Many_IList_None()
        {
            var res = new[] { 40, 42, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Many_None()
        {
            var res = new[] { 40, 42, 44 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Single_Pass()
        {
            var res = Return42.FirstOrDefaultAsync(x => x % 2 == 0);
            Assert.Equal(42, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Many_IList_Pass1()
        {
            var res = new[] { 42, 43, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(43, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Many_IList_Pass2()
        {
            var res = new[] { 42, 45, 90 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(45, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Many_Pass1()
        {
            var res = new[] { 42, 43, 44 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(43, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_Many_Pass2()
        {
            var res = new[] { 42, 45, 90 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync(x => x % 2 != 0);
            Assert.Equal(45, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_Predicate_PredicateThrows()
        {
            var res = new[] { 0, 1, 2 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => 1 / x > 0);
            await AssertThrowsAsync<DivideByZeroException>(res.AsTask());
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Empty()
        {
            var res = AsyncEnumerable.Empty<int>().FirstOrDefaultAsync(x => new ValueTask<bool>(true));
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Throw()
        {
            var ex = new Exception("Bang!");
            var res = Throw<int>(ex).FirstOrDefaultAsync(x => new ValueTask<bool>(true));
            await AssertThrowsAsync(res, ex);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Single_None()
        {
            var res = Return42.FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Many_IList_None()
        {
            var res = new[] { 40, 42, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Many_None()
        {
            var res = new[] { 40, 42, 44 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Single_Pass()
        {
            var res = Return42.FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 == 0));
            Assert.Equal(42, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Many_IList_Pass1()
        {
            var res = new[] { 42, 43, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(43, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Many_IList_Pass2()
        {
            var res = new[] { 42, 45, 90 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(45, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Many_Pass1()
        {
            var res = new[] { 42, 43, 44 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(43, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_Many_Pass2()
        {
            var res = new[] { 42, 45, 90 }.Select(x => x).ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(x % 2 != 0));
            Assert.Equal(45, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicate_AsyncPredicateThrows()
        {
            var res = new[] { 0, 1, 2 }.ToAsyncEnumerable().FirstOrDefaultAsync(x => new ValueTask<bool>(1 / x > 0));
            await AssertThrowsAsync<DivideByZeroException>(res.AsTask());
        }

#if !NO_DEEP_CANCELLATION
        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Empty()
        {
            var res = AsyncEnumerable.Empty<int>().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(true), CancellationToken.None);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Throw()
        {
            var ex = new Exception("Bang!");
            var res = Throw<int>(ex).FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(true), CancellationToken.None);
            await AssertThrowsAsync(res, ex);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Single_None()
        {
            var res = Return42.FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Many_IList_None()
        {
            var res = new[] { 40, 42, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Many_None()
        {
            var res = new[] { 40, 42, 44 }.Select((x, ct) => x).ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(0, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Single_Pass()
        {
            var res = Return42.FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 == 0), CancellationToken.None);
            Assert.Equal(42, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Many_IList_Pass1()
        {
            var res = new[] { 42, 43, 44 }.ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(43, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Many_IList_Pass2()
        {
            var res = new[] { 42, 45, 90 }.ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(45, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Many_Pass1()
        {
            var res = new[] { 42, 43, 44 }.Select((x, ct) => x).ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(43, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_Many_Pass2()
        {
            var res = new[] { 42, 45, 90 }.Select((x, ct) => x).ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(x % 2 != 0), CancellationToken.None);
            Assert.Equal(45, await res);
        }

        [Fact]
        public async Task FirstOrDefaultAsync_AsyncPredicateWithCancellation_AsyncPredicateWithCancellationThrows()
        {
            var res = new[] { 0, 1, 2 }.ToAsyncEnumerable().FirstOrDefaultAsync((x, ct) => new ValueTask<bool>(1 / x > 0), CancellationToken.None);
            await AssertThrowsAsync<DivideByZeroException>(res.AsTask());
        }
#endif
    }
}
