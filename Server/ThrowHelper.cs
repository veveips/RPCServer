using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Cryptography;
using System.Threading;

namespace RPCServer.Server
{
    // Token: 0x0200005B RID: 91
    internal static class ThrowHelper
    {
        // Token: 0x06000334 RID: 820 RVA: 0x00008089 File Offset: 0x00006289
        internal static void ThrowArgumentOutOfRangeException()
        {
            ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
        }

        // Token: 0x06000335 RID: 821 RVA: 0x00008094 File Offset: 0x00006294
        internal static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
        {
            throw new ArgumentException("Arg_WrongType");
        }

        // Token: 0x06000336 RID: 822 RVA: 0x000080B8 File Offset: 0x000062B8
        internal static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
        {
            throw new ArgumentException("Arg_WrongType");
        }

        // Token: 0x06000337 RID: 823 RVA: 0x000080DC File Offset: 0x000062DC
        internal static void ThrowKeyNotFoundException()
        {
            throw new KeyNotFoundException();
        }

        // Token: 0x06000338 RID: 824 RVA: 0x000080E3 File Offset: 0x000062E3
        internal static void ThrowArgumentException(ExceptionResource resource)
        {
            throw new ArgumentException(GetResourceName(resource));
        }

        // Token: 0x06000339 RID: 825 RVA: 0x000080F5 File Offset: 0x000062F5
        internal static void ThrowArgumentException(ExceptionResource resource, ExceptionArgument argument)
        {
            throw new ArgumentException(GetResourceName(resource));
        }

        // Token: 0x0600033A RID: 826 RVA: 0x0000810D File Offset: 0x0000630D
        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(GetArgumentName(argument));
        }

        // Token: 0x0600033B RID: 827 RVA: 0x0000811A File Offset: 0x0000631A
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        // Token: 0x0600033C RID: 828 RVA: 0x00008127 File Offset: 0x00006327
        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            throw new ArgumentOutOfRangeException(GetArgumentName(argument));
        }

        // Token: 0x0600033D RID: 829 RVA: 0x00008157 File Offset: 0x00006357
        internal static void ThrowInvalidOperationException(ExceptionResource resource)
        {
            throw new InvalidOperationException(GetResourceName(resource));
        }

        // Token: 0x0600033E RID: 830 RVA: 0x00008169 File Offset: 0x00006369
        internal static void ThrowSerializationException(ExceptionResource resource)
        {
            throw new SerializationException(GetResourceName(resource));
        }

        // Token: 0x0600033F RID: 831 RVA: 0x0000817B File Offset: 0x0000637B
        internal static void ThrowSecurityException(ExceptionResource resource)
        {
            throw new SecurityException(GetResourceName(resource));
        }

        // Token: 0x06000340 RID: 832 RVA: 0x0000818D File Offset: 0x0000638D
        internal static void ThrowNotSupportedException(ExceptionResource resource)
        {
            throw new NotSupportedException(GetResourceName(resource));
        }

        // Token: 0x06000341 RID: 833 RVA: 0x0000819F File Offset: 0x0000639F
        internal static void ThrowUnauthorizedAccessException(ExceptionResource resource)
        {
            throw new UnauthorizedAccessException(GetResourceName(resource));
        }

        // Token: 0x06000342 RID: 834 RVA: 0x000081B1 File Offset: 0x000063B1
        internal static void ThrowObjectDisposedException(string objectName, ExceptionResource resource)
        {
            throw new ObjectDisposedException(objectName, GetResourceName(resource));
        }

        // Token: 0x06000343 RID: 835 RVA: 0x000081C4 File Offset: 0x000063C4
        internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, ExceptionArgument argName)
        {
            if (value == null && default(T) != null)
            {
                ThrowArgumentNullException(argName);
            }
        }

        // Token: 0x06000344 RID: 836 RVA: 0x000081EC File Offset: 0x000063EC
        internal static string GetArgumentName(ExceptionArgument argument)
        {
            string result;
            switch (argument)
            {
                case ExceptionArgument.obj:
                    result = "obj";
                    break;
                case ExceptionArgument.dictionary:
                    result = "dictionary";
                    break;
                case ExceptionArgument.dictionaryCreationThreshold:
                    result = "dictionaryCreationThreshold";
                    break;
                case ExceptionArgument.array:
                    result = "array";
                    break;
                case ExceptionArgument.info:
                    result = "info";
                    break;
                case ExceptionArgument.key:
                    result = "key";
                    break;
                case ExceptionArgument.collection:
                    result = "collection";
                    break;
                case ExceptionArgument.list:
                    result = "list";
                    break;
                case ExceptionArgument.match:
                    result = "match";
                    break;
                case ExceptionArgument.converter:
                    result = "converter";
                    break;
                case ExceptionArgument.queue:
                    result = "queue";
                    break;
                case ExceptionArgument.stack:
                    result = "stack";
                    break;
                case ExceptionArgument.capacity:
                    result = "capacity";
                    break;
                case ExceptionArgument.index:
                    result = "index";
                    break;
                case ExceptionArgument.startIndex:
                    result = "startIndex";
                    break;
                case ExceptionArgument.value:
                    result = "value";
                    break;
                case ExceptionArgument.count:
                    result = "count";
                    break;
                case ExceptionArgument.arrayIndex:
                    result = "arrayIndex";
                    break;
                case ExceptionArgument.name:
                    result = "name";
                    break;
                case ExceptionArgument.mode:
                    result = "mode";
                    break;
                case ExceptionArgument.item:
                    result = "item";
                    break;
                case ExceptionArgument.options:
                    result = "options";
                    break;
                case ExceptionArgument.view:
                    result = "view";
                    break;
                case ExceptionArgument.sourceBytesToCopy:
                    result = "sourceBytesToCopy";
                    break;
                default:
                    return string.Empty;
            }
            return result;
        }

        // Token: 0x06000345 RID: 837 RVA: 0x00008348 File Offset: 0x00006548
        internal static string GetResourceName(ExceptionResource resource)
        {
            string result;
            switch (resource)
            {
                case ExceptionResource.Argument_ImplementIComparable:
                    result = "Argument_ImplementIComparable";
                    break;
                case ExceptionResource.Argument_InvalidType:
                    result = "Argument_InvalidType";
                    break;
                case ExceptionResource.Argument_InvalidArgumentForComparison:
                    result = "Argument_InvalidArgumentForComparison";
                    break;
                case ExceptionResource.Argument_InvalidRegistryKeyPermissionCheck:
                    result = "Argument_InvalidRegistryKeyPermissionCheck";
                    break;
                case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
                    result = "ArgumentOutOfRange_NeedNonNegNum";
                    break;
                case ExceptionResource.Arg_ArrayPlusOffTooSmall:
                    result = "Arg_ArrayPlusOffTooSmall";
                    break;
                case ExceptionResource.Arg_NonZeroLowerBound:
                    result = "Arg_NonZeroLowerBound";
                    break;
                case ExceptionResource.Arg_RankMultiDimNotSupported:
                    result = "Arg_RankMultiDimNotSupported";
                    break;
                case ExceptionResource.Arg_RegKeyDelHive:
                    result = "Arg_RegKeyDelHive";
                    break;
                case ExceptionResource.Arg_RegKeyStrLenBug:
                    result = "Arg_RegKeyStrLenBug";
                    break;
                case ExceptionResource.Arg_RegSetStrArrNull:
                    result = "Arg_RegSetStrArrNull";
                    break;
                case ExceptionResource.Arg_RegSetMismatchedKind:
                    result = "Arg_RegSetMismatchedKind";
                    break;
                case ExceptionResource.Arg_RegSubKeyAbsent:
                    result = "Arg_RegSubKeyAbsent";
                    break;
                case ExceptionResource.Arg_RegSubKeyValueAbsent:
                    result = "Arg_RegSubKeyValueAbsent";
                    break;
                case ExceptionResource.Argument_AddingDuplicate:
                    result = "Argument_AddingDuplicate";
                    break;
                case ExceptionResource.Serialization_InvalidOnDeser:
                    result = "Serialization_InvalidOnDeser";
                    break;
                case ExceptionResource.Serialization_MissingKeys:
                    result = "Serialization_MissingKeys";
                    break;
                case ExceptionResource.Serialization_NullKey:
                    result = "Serialization_NullKey";
                    break;
                case ExceptionResource.Argument_InvalidArrayType:
                    result = "Argument_InvalidArrayType";
                    break;
                case ExceptionResource.NotSupported_KeyCollectionSet:
                    result = "NotSupported_KeyCollectionSet";
                    break;
                case ExceptionResource.NotSupported_ValueCollectionSet:
                    result = "NotSupported_ValueCollectionSet";
                    break;
                case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
                    result = "ArgumentOutOfRange_SmallCapacity";
                    break;
                case ExceptionResource.ArgumentOutOfRange_Index:
                    result = "ArgumentOutOfRange_Index";
                    break;
                case ExceptionResource.Argument_InvalidOffLen:
                    result = "Argument_InvalidOffLen";
                    break;
                case ExceptionResource.Argument_ItemNotExist:
                    result = "Argument_ItemNotExist";
                    break;
                case ExceptionResource.ArgumentOutOfRange_Count:
                    result = "ArgumentOutOfRange_Count";
                    break;
                case ExceptionResource.ArgumentOutOfRange_InvalidThreshold:
                    result = "ArgumentOutOfRange_InvalidThreshold";
                    break;
                case ExceptionResource.ArgumentOutOfRange_ListInsert:
                    result = "ArgumentOutOfRange_ListInsert";
                    break;
                case ExceptionResource.NotSupported_ReadOnlyCollection:
                    result = "NotSupported_ReadOnlyCollection";
                    break;
                case ExceptionResource.InvalidOperation_CannotRemoveFromStackOrQueue:
                    result = "InvalidOperation_CannotRemoveFromStackOrQueue";
                    break;
                case ExceptionResource.InvalidOperation_EmptyQueue:
                    result = "InvalidOperation_EmptyQueue";
                    break;
                case ExceptionResource.InvalidOperation_EnumOpCantHappen:
                    result = "InvalidOperation_EnumOpCantHappen";
                    break;
                case ExceptionResource.InvalidOperation_EnumFailedVersion:
                    result = "InvalidOperation_EnumFailedVersion";
                    break;
                case ExceptionResource.InvalidOperation_EmptyStack:
                    result = "InvalidOperation_EmptyStack";
                    break;
                case ExceptionResource.ArgumentOutOfRange_BiggerThanCollection:
                    result = "ArgumentOutOfRange_BiggerThanCollection";
                    break;
                case ExceptionResource.InvalidOperation_EnumNotStarted:
                    result = "InvalidOperation_EnumNotStarted";
                    break;
                case ExceptionResource.InvalidOperation_EnumEnded:
                    result = "InvalidOperation_EnumEnded";
                    break;
                case ExceptionResource.NotSupported_SortedListNestedWrite:
                    result = "NotSupported_SortedListNestedWrite";
                    break;
                case ExceptionResource.InvalidOperation_NoValue:
                    result = "InvalidOperation_NoValue";
                    break;
                case ExceptionResource.InvalidOperation_RegRemoveSubKey:
                    result = "InvalidOperation_RegRemoveSubKey";
                    break;
                case ExceptionResource.Security_RegistryPermission:
                    result = "Security_RegistryPermission";
                    break;
                case ExceptionResource.UnauthorizedAccess_RegistryNoWrite:
                    result = "UnauthorizedAccess_RegistryNoWrite";
                    break;
                case ExceptionResource.ObjectDisposed_RegKeyClosed:
                    result = "ObjectDisposed_RegKeyClosed";
                    break;
                case ExceptionResource.NotSupported_InComparableType:
                    result = "NotSupported_InComparableType";
                    break;
                case ExceptionResource.Argument_InvalidRegistryOptionsCheck:
                    result = "Argument_InvalidRegistryOptionsCheck";
                    break;
                case ExceptionResource.Argument_InvalidRegistryViewCheck:
                    result = "Argument_InvalidRegistryViewCheck";
                    break;
                default:
                    return string.Empty;
            }
            return result;
        }
    }
    // Token: 0x0200005C RID: 92
    internal enum ExceptionArgument
    {
        // Token: 0x040001F2 RID: 498
        obj,
        // Token: 0x040001F3 RID: 499
        dictionary,
        // Token: 0x040001F4 RID: 500
        dictionaryCreationThreshold,
        // Token: 0x040001F5 RID: 501
        array,
        // Token: 0x040001F6 RID: 502
        info,
        // Token: 0x040001F7 RID: 503
        key,
        // Token: 0x040001F8 RID: 504
        collection,
        // Token: 0x040001F9 RID: 505
        list,
        // Token: 0x040001FA RID: 506
        match,
        // Token: 0x040001FB RID: 507
        converter,
        // Token: 0x040001FC RID: 508
        queue,
        // Token: 0x040001FD RID: 509
        stack,
        // Token: 0x040001FE RID: 510
        capacity,
        // Token: 0x040001FF RID: 511
        index,
        // Token: 0x04000200 RID: 512
        startIndex,
        // Token: 0x04000201 RID: 513
        value,
        // Token: 0x04000202 RID: 514
        count,
        // Token: 0x04000203 RID: 515
        arrayIndex,
        // Token: 0x04000204 RID: 516
        name,
        // Token: 0x04000205 RID: 517
        mode,
        // Token: 0x04000206 RID: 518
        item,
        // Token: 0x04000207 RID: 519
        options,
        // Token: 0x04000208 RID: 520
        view,
        // Token: 0x04000209 RID: 521
        sourceBytesToCopy
    }
    // Token: 0x0200005D RID: 93
    internal enum ExceptionResource
    {
        // Token: 0x0400020B RID: 523
        Argument_ImplementIComparable,
        // Token: 0x0400020C RID: 524
        Argument_InvalidType,
        // Token: 0x0400020D RID: 525
        Argument_InvalidArgumentForComparison,
        // Token: 0x0400020E RID: 526
        Argument_InvalidRegistryKeyPermissionCheck,
        // Token: 0x0400020F RID: 527
        ArgumentOutOfRange_NeedNonNegNum,
        // Token: 0x04000210 RID: 528
        Arg_ArrayPlusOffTooSmall,
        // Token: 0x04000211 RID: 529
        Arg_NonZeroLowerBound,
        // Token: 0x04000212 RID: 530
        Arg_RankMultiDimNotSupported,
        // Token: 0x04000213 RID: 531
        Arg_RegKeyDelHive,
        // Token: 0x04000214 RID: 532
        Arg_RegKeyStrLenBug,
        // Token: 0x04000215 RID: 533
        Arg_RegSetStrArrNull,
        // Token: 0x04000216 RID: 534
        Arg_RegSetMismatchedKind,
        // Token: 0x04000217 RID: 535
        Arg_RegSubKeyAbsent,
        // Token: 0x04000218 RID: 536
        Arg_RegSubKeyValueAbsent,
        // Token: 0x04000219 RID: 537
        Argument_AddingDuplicate,
        // Token: 0x0400021A RID: 538
        Serialization_InvalidOnDeser,
        // Token: 0x0400021B RID: 539
        Serialization_MissingKeys,
        // Token: 0x0400021C RID: 540
        Serialization_NullKey,
        // Token: 0x0400021D RID: 541
        Argument_InvalidArrayType,
        // Token: 0x0400021E RID: 542
        NotSupported_KeyCollectionSet,
        // Token: 0x0400021F RID: 543
        NotSupported_ValueCollectionSet,
        // Token: 0x04000220 RID: 544
        ArgumentOutOfRange_SmallCapacity,
        // Token: 0x04000221 RID: 545
        ArgumentOutOfRange_Index,
        // Token: 0x04000222 RID: 546
        Argument_InvalidOffLen,
        // Token: 0x04000223 RID: 547
        Argument_ItemNotExist,
        // Token: 0x04000224 RID: 548
        ArgumentOutOfRange_Count,
        // Token: 0x04000225 RID: 549
        ArgumentOutOfRange_InvalidThreshold,
        // Token: 0x04000226 RID: 550
        ArgumentOutOfRange_ListInsert,
        // Token: 0x04000227 RID: 551
        NotSupported_ReadOnlyCollection,
        // Token: 0x04000228 RID: 552
        InvalidOperation_CannotRemoveFromStackOrQueue,
        // Token: 0x04000229 RID: 553
        InvalidOperation_EmptyQueue,
        // Token: 0x0400022A RID: 554
        InvalidOperation_EnumOpCantHappen,
        // Token: 0x0400022B RID: 555
        InvalidOperation_EnumFailedVersion,
        // Token: 0x0400022C RID: 556
        InvalidOperation_EmptyStack,
        // Token: 0x0400022D RID: 557
        ArgumentOutOfRange_BiggerThanCollection,
        // Token: 0x0400022E RID: 558
        InvalidOperation_EnumNotStarted,
        // Token: 0x0400022F RID: 559
        InvalidOperation_EnumEnded,
        // Token: 0x04000230 RID: 560
        NotSupported_SortedListNestedWrite,
        // Token: 0x04000231 RID: 561
        InvalidOperation_NoValue,
        // Token: 0x04000232 RID: 562
        InvalidOperation_RegRemoveSubKey,
        // Token: 0x04000233 RID: 563
        Security_RegistryPermission,
        // Token: 0x04000234 RID: 564
        UnauthorizedAccess_RegistryNoWrite,
        // Token: 0x04000235 RID: 565
        ObjectDisposed_RegKeyClosed,
        // Token: 0x04000236 RID: 566
        NotSupported_InComparableType,
        // Token: 0x04000237 RID: 567
        Argument_InvalidRegistryOptionsCheck,
        // Token: 0x04000238 RID: 568
        Argument_InvalidRegistryViewCheck
    }

    // Token: 0x0200046B RID: 1131
    //[FriendAccessAllowed]
    internal static class HashHelpers
    {
        // Token: 0x17000851 RID: 2129
        // (get) Token: 0x06003773 RID: 14195 RVA: 0x000D4F8C File Offset: 0x000D318C
        internal static ConditionalWeakTable<object, SerializationInfo> SerializationInfoTable
        {
            get
            {
                if (s_SerializationInfoTable == null)
                {
                    ConditionalWeakTable<object, SerializationInfo> value = new ConditionalWeakTable<object, SerializationInfo>();
                    Interlocked.CompareExchange(ref s_SerializationInfoTable, value, null);
                }
                return s_SerializationInfoTable;
            }
        }

        // Token: 0x06003774 RID: 14196 RVA: 0x000D4FB8 File Offset: 0x000D31B8
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static bool IsPrime(int candidate)
        {
            if ((candidate & 1) != 0)
            {
                int num = (int)Math.Sqrt(candidate);
                for (int i = 3; i <= num; i += 2)
                {
                    if (candidate % i == 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            return candidate == 2;
        }

        // Token: 0x06003775 RID: 14197 RVA: 0x000D4FEC File Offset: 0x000D31EC
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        public static int GetPrime(int min)
        {
            if (min < 0)
            {
                throw new ArgumentException("Arg_HTCapacityOverflow");
            }
            for (int i = 0; i < primes.Length; i++)
            {
                int num = primes[i];
                if (num >= min)
                {
                    return num;
                }
            }
            for (int j = min | 1; j < 2147483647; j += 2)
            {
                if (IsPrime(j) && (j - 1) % 101 != 0)
                {
                    return j;
                }
            }
            return min;
        }

        // Token: 0x06003776 RID: 14198 RVA: 0x000D5052 File Offset: 0x000D3252
        public static int GetMinPrime()
        {
            return primes[0];
        }

        // Token: 0x06003777 RID: 14199 RVA: 0x000D505C File Offset: 0x000D325C
        public static int ExpandPrime(int oldSize)
        {
            int num = 2 * oldSize;
            if (num > 2146435069 && 2146435069 > oldSize)
            {
                return 2146435069;
            }
            return GetPrime(num);
        }

        // Token: 0x06003778 RID: 14200 RVA: 0x000D5089 File Offset: 0x000D3289
        public static bool IsWellKnownEqualityComparer(object comparer)
        {
            return comparer == null || comparer == EqualityComparer<string>.Default || comparer is IWellKnownStringEqualityComparer;
        }

        // Token: 0x06003779 RID: 14201 RVA: 0x000D50A4 File Offset: 0x000D32A4
        public static global::System.Collections.IEqualityComparer GetRandomizedEqualityComparer(object comparer)
        {
            if (comparer == null)
            {
                return null;
            }
            if (comparer == EqualityComparer<string>.Default)
            {
                return null;
            }
            IWellKnownStringEqualityComparer wellKnownStringEqualityComparer = comparer as IWellKnownStringEqualityComparer;
            if (wellKnownStringEqualityComparer != null)
            {
                return wellKnownStringEqualityComparer.GetRandomizedEqualityComparer();
            }
            return null;
        }

        // Token: 0x0600377A RID: 14202 RVA: 0x000D50DC File Offset: 0x000D32DC
        public static object GetEqualityComparerForSerialization(object comparer)
        {
            if (comparer == null)
            {
                return null;
            }
            IWellKnownStringEqualityComparer wellKnownStringEqualityComparer = comparer as IWellKnownStringEqualityComparer;
            if (wellKnownStringEqualityComparer != null)
            {
                return wellKnownStringEqualityComparer.GetEqualityComparerForSerialization();
            }
            return comparer;
        }

        // Token: 0x0600377B RID: 14203 RVA: 0x000D5100 File Offset: 0x000D3300
        internal static long GetEntropy()
        {
            object obj = lockObj;
            long result;
            lock (obj)
            {
                if (currentIndex == 1024)
                {
                    if (rng == null)
                    {
                        rng = RandomNumberGenerator.Create();
                        data = new byte[1024];
                    }
                    rng.GetBytes(data);
                    currentIndex = 0;
                }
                long num = BitConverter.ToInt64(data, currentIndex);
                currentIndex += 8;
                result = num;
            }
            return result;
        }

        // Token: 0x04001839 RID: 6201
        public const int HashCollisionThreshold = 100;

        // Token: 0x0400183A RID: 6202
        //public static bool s_UseRandomizedStringHashing = string.UseRandomizedHashing();

        // Token: 0x0400183B RID: 6203
        public static readonly int[] primes = new int[]
        {
            3,
            7,
            11,
            17,
            23,
            29,
            37,
            47,
            59,
            71,
            89,
            107,
            131,
            163,
            197,
            239,
            293,
            353,
            431,
            521,
            631,
            761,
            919,
            1103,
            1327,
            1597,
            1931,
            2333,
            2801,
            3371,
            4049,
            4861,
            5839,
            7013,
            8419,
            10103,
            12143,
            14591,
            17519,
            21023,
            25229,
            30293,
            36353,
            43627,
            52361,
            62851,
            75431,
            90523,
            108631,
            130363,
            156437,
            187751,
            225307,
            270371,
            324449,
            389357,
            467237,
            560689,
            672827,
            807403,
            968897,
            1162687,
            1395263,
            1674319,
            2009191,
            2411033,
            2893249,
            3471899,
            4166287,
            4999559,
            5999471,
            7199369
        };

        // Token: 0x0400183C RID: 6204
        private static ConditionalWeakTable<object, SerializationInfo> s_SerializationInfoTable;

        // Token: 0x0400183D RID: 6205
        public const int MaxPrimeArrayLength = 2146435069;

        // Token: 0x0400183E RID: 6206
        private const int bufferSize = 1024;

        // Token: 0x0400183F RID: 6207
        private static RandomNumberGenerator rng;

        // Token: 0x04001840 RID: 6208
        private static byte[] data;

        // Token: 0x04001841 RID: 6209
        private static int currentIndex = 1024;

        // Token: 0x04001842 RID: 6210
        private static readonly object lockObj = new object();
    }

    // Token: 0x0200007A RID: 122
    internal interface IWellKnownStringEqualityComparer
    {
        // Token: 0x060005A8 RID: 1448
        IEqualityComparer GetRandomizedEqualityComparer();

        // Token: 0x060005A9 RID: 1449
        IEqualityComparer GetEqualityComparerForSerialization();
    }

    public static class TypeExtend
    {
        public static Type[] GetInterfaceGenericTypeArguments(this Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var genericTypeArguments = interfaceType.GetGenericArguments();
                if (genericTypeArguments == null)
                    continue;
                if (genericTypeArguments.Length == 0)
                    continue;
                return genericTypeArguments;
            }
            throw new Exception("获取接口失败!");
        }

        public static Type GetArrayItemType(this Type type)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                var genericTypeArguments = interfaceType.GetGenericArguments();
                if (genericTypeArguments == null)
                    continue;
                if (genericTypeArguments.Length == 0)
                    continue;
                return genericTypeArguments[0];
            }
            return null;
        }

        public static bool IsInterfaceType(this Type type, Type interfaceType)
        {
            foreach (var interfaceType1 in type.GetInterfaces())
            {
                if (interfaceType1 == interfaceType)
                    return true;
            }
            return false;
        }
    }
}
