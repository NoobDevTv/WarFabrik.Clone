#nullable disable
namespace BotMaster.Betterplace.MessageContract
{
    [global::System.Diagnostics.DebuggerTypeProxy(typeof(BetterplaceMessage.__DebuggerTypeProxy))]
    [global::System.Diagnostics.DebuggerDisplay("{_variant.AsObject}", Type = "{_variant.TypeString,nq}")]
    partial class BetterplaceMessage
        : global::System.IEquatable<BetterplaceMessage>
    {
        private readonly global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage _variant;

        /// <summary>
        /// Create a BetterplaceMessage with a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.
        /// </summary>
        /// <param name="donation">The value to initlaize the variant with.</param>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public BetterplaceMessage(global::BotMaster.Betterplace.MessageContract.Donation donation)
            => _variant = new global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage(donation);

        /// <summary>
        /// Create a BetterplaceMessage with a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.
        /// </summary>
        /// <param name="donation">The value to initlaize the variant with.</param>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static implicit operator BetterplaceMessage(global::BotMaster.Betterplace.MessageContract.Donation donation)
            => new BetterplaceMessage(donation);

        /// <summary>
        /// Create a BetterplaceMessage with a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.
        /// </summary>
        /// <param name="donation">The value to initlaize the variant with.</param>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static BetterplaceMessage Create(global::BotMaster.Betterplace.MessageContract.Donation donation)
            => new BetterplaceMessage(donation);


        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.IsEmpty"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool IsEmpty
            => _variant.IsEmpty;

        /// <inheritdoc/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public override bool Equals(object other)
            => other is BetterplaceMessage v
            && Equals(v);

        /// <inheritdoc/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool Equals(BetterplaceMessage other)
            => !(other is null) && _variant.Equals(other._variant);

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static bool operator ==(BetterplaceMessage lhs, BetterplaceMessage rhs)
            => lhs?.Equals(rhs) ?? (rhs is null);

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static bool operator !=(BetterplaceMessage lhs, BetterplaceMessage rhs)
            => !(lhs == rhs);

        [global::System.Diagnostics.DebuggerNonUserCode]
        public override int GetHashCode()
            => _variant.GetHashCode();

        [global::System.Diagnostics.DebuggerNonUserCode]
        public override string ToString()
            => _variant.ToString();

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.TryMatch(out global::BotMaster.Betterplace.MessageContract.Donation)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool TryMatch(out global::BotMaster.Betterplace.MessageContract.Donation donation)
            => _variant.TryMatch(out donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.TryMatch(global::System.Action{global::BotMaster.Betterplace.MessageContract.Donation})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool TryMatch(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation)
            => _variant.TryMatch(donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Match(out global::BotMaster.Betterplace.MessageContract.Donation)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Match(out global::BotMaster.Betterplace.MessageContract.Donation donation)
            => _variant.Match(out donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Match(global::System.Action{global::BotMaster.Betterplace.MessageContract.Donation})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Match(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation)
            => _variant.Match(donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Match(global::System.Action{global::BotMaster.Betterplace.MessageContract.Donation}, global::System.Action)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Match(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation, global::System.Action _)
            => _variant.Match(donation, _);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Match{TResult}(global::System.Func{global::BotMaster.Betterplace.MessageContract.Donation, TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Match<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
            => _variant.Match(donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Match{TResult}(global::System.Func{global::BotMaster.Betterplace.MessageContract.Donation, TResult}, TResult)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Match<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation, TResult _)
            => _variant.Match(donation, _);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Match{TResult}(global::System.Func{global::BotMaster.Betterplace.MessageContract.Donation, TResult}, global::System.Func{TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Match<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation, global::System.Func<TResult> _)
            => _variant.Match(donation, _);


        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Visit(global::System.Action{global::BotMaster.Betterplace.MessageContract.Donation})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Visit(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation)
            => _variant.Visit(donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Visit(global::System.Action{global::BotMaster.Betterplace.MessageContract.Donation}, global::System.Action)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Visit(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation, global::System.Action _)
            => _variant.Visit(donation, _);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Visit{TResult}(global::System.Func{global::BotMaster.Betterplace.MessageContract.Donation, TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
            => _variant.Visit(donation);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.Betterplace.MessageContract.BetterplaceMessage.Visit{TResult}(global::System.Func{global::BotMaster.Betterplace.MessageContract.Donation, TResult}, global::System.Func{TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation, global::System.Func<TResult> _)
            => _variant.Visit(donation, _);

        private sealed class __DebuggerTypeProxy
        {
            public object Value { get; }
            public __DebuggerTypeProxy(BetterplaceMessage v)
            {
                Value = v._variant.AsObject;
                #pragma warning disable 8604 // Possible null reference argument for parameter
                #pragma warning disable 8625 // Cannot convert null literal to non-nullable reference type
                VariantOf(default);
                #pragma warning restore 8604, 8625
            }
        }

        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static explicit operator global::dotVariant.GeneratorSupport.Discriminator(BetterplaceMessage v)
            => (global::dotVariant.GeneratorSupport.Discriminator)v._variant;
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static explicit operator global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>(BetterplaceMessage v)
            => (global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)v._variant;
    }
}

namespace dotVariant._G.BotMaster.Betterplace.MessageContract
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.DebuggerDisplay("{AsObject}", Type = "{TypeString,nq}")]
    internal readonly struct BetterplaceMessage
    {
        private readonly struct Union
        {
            public readonly global::BotMaster.Betterplace.MessageContract.Donation _1;
            public Union(global::BotMaster.Betterplace.MessageContract.Donation value)
            {
                _1 = value;
            }
        }

        private readonly Union _x;
        private readonly byte _n;

        public BetterplaceMessage(global::BotMaster.Betterplace.MessageContract.Donation donation)
        {
            _n = 1;
            _x = new Union(donation);
        }


        public static explicit operator global::dotVariant.GeneratorSupport.Discriminator(in BetterplaceMessage v)
            => (global::dotVariant.GeneratorSupport.Discriminator)v._n;
        public static explicit operator global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>(in BetterplaceMessage v)
            => new global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>(v._x._1);

        /// <summary>
        /// <see langword="true"/> if BetterplaceMessage was constructed without a value.
        /// </summary>
        public bool IsEmpty => _n == 0;

        /// <summary>
        /// The string representation of the stored value's type.
        /// </summary>
        public string TypeString
        {
            get
            {
                switch (_n)
                {
                    case 0:
                        return "<empty>";
                    case 1:
                        return "BotMaster.Betterplace.MessageContract.Donation";
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<string>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                }
            }
        }

        /// <summary>
        /// The stored value's <see cref="object.ToString()"/> result, or <c>""</c> if empty.
        /// </summary>
        public override string ToString()
        {
            switch (_n)
            {
                case 0:
                    return "";
                case 1:
                    return _x._1?.ToString() ?? "null";
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<string>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
            }
        }

        /// <summary>
        /// The stored value cast to type <see cref="object"/>.
        /// </summary>
        public object AsObject
        {
            get
            {
                switch (_n)
                {
                    case 0:
                        return null;
                    case 1:
                        return _x._1;
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<object>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                }
            }
        }

        public bool Equals(in BetterplaceMessage other)
        {
            if (_n != other._n)
            {
                return false;
            }
            switch (_n)
            {
                case 0:
                    return true;
                case 1:
                    return global::System.Collections.Generic.EqualityComparer<global::BotMaster.Betterplace.MessageContract.Donation>.Default.Equals(_x._1, other._x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<bool>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
            }
        }

        public override int GetHashCode()
        {
            switch (_n)
            {
                case 0:
                    return 0;
                case 1:
                    return global::System.HashCode.Combine(_x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<int>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
            }
        }

        /// <summary>
        /// Retrieve the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.
        /// </summary>
        /// <param name="donation">Receives the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <returns><see langword="true"/> if BetterplaceMessage contained a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</returns>
        public bool TryMatch(out global::BotMaster.Betterplace.MessageContract.Donation donation)
        {
            donation = _n == 1 ? _x._1 : default;
            return _n == 1;
        }

        /// <summary>
        /// Invoke a delegate with the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.
        /// </summary>
        /// <param name="donation">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <returns><see langword="true"/> if BetterplaceMessage contained a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="donation"> is rethrown.</exception>
        public bool TryMatch(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation)
        {
            if (_n == 1)
            {
                donation(_x._1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieve the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>,
        /// otherwise throw <see cref="global::System.InvalidOperationException"/>.
        /// </summary>
        /// <param name="donation">Receives the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">BetterplaceMessage does not contain a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</exception>
        public void Match(out global::BotMaster.Betterplace.MessageContract.Donation donation)
        {
            if (_n == 1)
            {
                donation = _x._1;
                return;
            }
            throw global::dotVariant.GeneratorSupport.Errors.MakeMismatchError("BotMaster.Betterplace.MessageContract.BetterplaceMessage", "BotMaster.Betterplace.MessageContract.Donation", TypeString);
        }

        /// <summary>
        /// Invoke a delegate with the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>,
        /// otherwise throw <see cref="global::System.InvalidOperationException"/>.
        /// </summary>
        /// <param name="donation">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">BetterplaceMessage does not contain a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="donation"/> is rethrown.</exception>
        public void Match(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation)
        {
            if (_n == 1)
            {
                donation(_x._1);
                return;
            }
            global::dotVariant.GeneratorSupport.Errors.ThrowMismatchError("BotMaster.Betterplace.MessageContract.BetterplaceMessage", "BotMaster.Betterplace.MessageContract.Donation", TypeString);
        }

        /// <summary>
        /// Invoke a delegate with the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>,
        /// otherwise invoke an alternative delegate.
        /// </summary>
        /// <param name="donation">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The delegate to invoke if the stored value is of a different type.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="donation"/> or <paramref name="_"/> is rethrown.</exception>
        public void Match(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation, global::System.Action _)
        {
            if (_n == 1)
            {
                donation(_x._1);
            }
            else
            {
                _();
            }
        }

        /// <summary>
        /// Invoke a delegate with the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> and return the result,
        /// otherwise throw <see cref="global::System.InvalidOperationException"/>.
        /// </summary>
        /// <param name="donation">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <returns>The value returned from invoking <paramref name="donation"/>.</returns>
        /// <exception cref="global::System.InvalidOperationException">BetterplaceMessage does not contain a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="donation"/> is rethrown.</exception>
        public TResult Match<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
        {
            if (_n == 1)
            {
                return donation(_x._1);
            }
            return global::dotVariant.GeneratorSupport.Errors.ThrowMismatchError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage", "BotMaster.Betterplace.MessageContract.Donation", TypeString);
        }

        /// <summary>
        /// Invoke a delegate with the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> and return the result,
        /// otherwise return a provided value.
        /// </summary>
        /// <param name="donation">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The value to return if the stored value is of a different type.</param>
        /// <returns>The value returned from invoking <paramref name="donation"/>, or <paramref name="default"/>.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="donation"/> or <paramref name="_"/> is rethrown.</exception>
        public TResult Match<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation, TResult _)
        {
            return _n == 1 ? donation(_x._1) : _;
        }

        /// <summary>
        /// Invoke a delegate with the value stored within BetterplaceMessage if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> and return the result,
        /// otherwise invoke an alternative delegate and return its result.
        /// </summary>
        /// <param name="donation">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The delegate to invoke if the stored value is of a different type.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="donation"/> or <paramref name="_"/> is rethrown.</exception>
        public TResult Match<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation, global::System.Func<TResult> _)
        {
            return _n == 1 ? donation(_x._1) : _();
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of type of the value stored within BetterplaceMessage,
        /// and invoke a special delegate if BetterplaceMessage is empty.
        /// </summary>
        /// <param name="donation">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The delegate to invoke if BetterplaceMessage is empty.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        public void Visit(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation, global::System.Action _)
        {
            switch (_n)
            {
                case 0:
                    _();
                    break;
                case 1:
                    donation(_x._1);
                    break;
                default:
                    global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                    break;
            }
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of the value stored within BetterplaceMessage,
        /// and throw an exception if BetterplaceMessage is empty.
        /// </summary>
        /// <param name="donation">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">BetterplaceMessage is empty.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        public void Visit(global::System.Action<global::BotMaster.Betterplace.MessageContract.Donation> donation)
        {
            switch (_n)
            {
                case 0:
                    global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                    break;
                case 1:
                    donation(_x._1);
                    break;
                default:
                    global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                    break;
            }
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of type of the value stored within BetterplaceMessage and return the result,
        /// and invoke a special delegate if BetterplaceMessage is empty and return its result.
        /// </summary>
        /// <param name="donation">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The delegate to invoke if BetterplaceMessage is empty.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The return type of all delegates, and by extension the return type of this function.</typeparam>
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation, global::System.Func<TResult> _)
        {
            switch (_n)
            {
                case 0:
                    return _();
                case 1:
                    return donation(_x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
            }
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of the value stored within BetterplaceMessage and return the result,
        /// and throw an exception if BetterplaceMessage is empty.
        /// </summary>
        /// <param name="donation">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">BetterplaceMessage is empty.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The return type of all delegates, and by extension the return type of this function.</typeparam>
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
        {
            switch (_n)
            {
                case 0:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                case 1:
                    return donation(_x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
            }
        }
    }
}


namespace BotMaster.Betterplace.MessageContract
{
    public static partial class BetterplaceMessageEx
    {
        /// <summary>
        /// Transform a BetterplaceMessage-based enumerable sequence by applying a selector function to those elements
        /// containing a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> and dropping all others.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="donation">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Match<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
        {
            foreach (var variant in source)
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant) == 1)
                {
                    yield return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)variant).Value);
                }
            }
        }

        /// <summary>
        /// Transform a BetterplaceMessage-based enumerable sequence by applying a selector function to those elements
        /// containing a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> and replacing all others by a fallback value.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="donation">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Match<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation,
                TResult _)
        {
            foreach (var variant in source)
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant) == 1)
                {
                    yield return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)variant).Value);
                }
                else
                {
                    yield return _;
                }
            }
        }

        /// <summary>
        /// Transform a BetterplaceMessage-based enumerable sequence by applying a selector function to those elements
        /// containing a value of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> and replacing all others with the result of a fallback selector.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="donation">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Match<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation,
                global::System.Func<TResult> _)
        {
            foreach (var variant in source)
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant) == 1)
                {
                    yield return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)variant).Value);
                }
                else
                {
                    yield return _();
                }
            }
        }

        /// <summary>
        /// Transform a BetterplaceMessage-based enumerable sequence by applying a selector function to each element
        /// wich matches the type stored within the value, and throwing <see cref="global::System.InvalidOperationException"/>
        /// if an element is empty.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="donation">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.InvalidOperationException">The sequence encountered an empty BetterplaceMessage.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Visit<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
        {
            foreach (var variant in source)
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant))
                {
                    case 0:
                        global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                        yield break;
                    case 1:
                        yield return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)variant).Value);
                        break;
                    default:
                        global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                        yield break;
                }
            }
        }

        /// <summary>
        /// Transform a BetterplaceMessage-based enumerable sequence by applying a selector function to each element
        /// wich matches the type stored within the value, and replacing empty elements with the result of a fallback selector.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="donation">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The delegate to invoke if an element is empty.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Visit<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation,
                global::System.Func<TResult> _)
        {
            foreach (var variant in source)
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant))
                {
                    case 0:
                        yield return _();
                        break;
                    case 1:
                        yield return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)variant).Value);
                        break;
                    default:
                        global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                        yield break;
                }
            }
        }
    }
}
namespace BotMaster.Betterplace.MessageContract
{
    public static partial class BetterplaceMessageEx
    {
        /// <summary>
        /// Projects each <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> element of an observable sequence
        /// into a new form and drops all other elements.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to match on.</param>
        /// <param name="donation">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <returns>An observable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Match<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
        {
            return global::System.Reactive.Linq.Observable.Select(
                global::System.Reactive.Linq.Observable.Where(source, _variant => ((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant) == 1),
                _variant => donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)_variant).Value));
        }

        /// <summary>
        /// Projects each <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> element of an observable sequence
        /// into a new form and replaces all other elements by a fallback value.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to match on.</param>
        /// <param name="donation">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An observable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Match<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation,
                TResult _)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant) == 1)
                {
                    return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)_variant).Value);
                }
                else
                {
                    return _;
                }
            });
        }

        /// <summary>
        /// Projects each <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> element of an observable sequence
        /// into a new form and replaces all other elements by a fallback selector result.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to match on.</param>
        /// <param name="donation">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An observable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Match<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation,
                global::System.Func<TResult> _)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant) == 1)
                {
                    return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)_variant).Value);
                }
                else
                {
                    return _();
                }
            });
        }

        /// <summary>
        /// Projects each element of an observable sequence into a new form depending on its contained value type,
        /// failing with <see cref="global::System.InvalidOperationException"/> if an element is empty.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to visit.</param>
        /// <param name="donation">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <returns>An observable sequence that contains the transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Visit<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant))
                {
                    case 0:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                    case 1:
                        return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)_variant).Value);
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                }
            });
        }

        /// <summary>
        /// Projects each element of an observable sequence into a new form depending on its contained value type,
        /// failing with <see cref="global::System.InvalidOperationException"/> if an element is empty.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to visit.</param>
        /// <param name="donation">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/>.</param>
        /// <param name="_">The delegate to invoke if an element is empty.</param>
        /// <returns>An observable sequence that contains the transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Visit<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::BotMaster.Betterplace.MessageContract.Donation, TResult> donation,
                global::System.Func<TResult> _)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant))
                {
                    case 0:
                        return _();
                    case 1:
                        return donation(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)_variant).Value);
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.Betterplace.MessageContract.BetterplaceMessage");
                }
            });
        }


        /// <summary>
        /// Splits the observable sequence of BetterplaceMessage elements into one independent sub-sequences per value type,
        /// transforming each sub-sequence by the provided selector, and merges the resulting values into one observable sequence.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>While the subscription to the source is active the sub-sequences are hot.</item>
        ///     <item>Multiple subscriptions and repeated subscriptions within the sub-sequences will not cause repeated subscriptions to the source.</item>
        ///     <item>Once the source sequence terminates it cannot be re-subscribed to with operators like <c>Repeat</c> or <c>Retry</c> from within a sub-sequence.</item>
        ///     <item>The first sub-sequence to produce an OnError message terminates the resulting sequence with OnError.</item>
        ///     <item>When all sub-sequences terminate with OnCompleted (even before the source does) the resulting sequence terminates.</item>
        /// </list>
        /// </remarks>
        /// <param name="source">An observable sequence whose elements to split into sub-sequences.</param>
        /// <param name="donation">Transform an observable sequence of <see cref="global::BotMaster.Betterplace.MessageContract.Donation"/> values into an observable sequence of <typeparamref name="TResult"/> values.</param>
        /// <param name="_">Transform a sequence of <see cref="global::System.Reactive.Unit"/> values (each representing an empty variant) into a sequence of <typeparamref name="TResult"/> values.</param>
        /// <returns>An observable sequence that contains the elements of all sub-sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            VisitMany<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::System.IObservable<global::BotMaster.Betterplace.MessageContract.Donation>, global::System.IObservable<TResult>> donation,
                global::System.Func<global::System.IObservable<global::System.Reactive.Unit>, global::System.IObservable<TResult>> _)
        {
            return VisitMany(source, (_1, _0) =>
            {
                return global::System.Reactive.Linq.Observable.Merge(donation(_1), _(_0));
            });
        }

        /// <summary>
        /// Splits the observable sequence of BetterplaceMessage elements into one independent sub-sequences per value type,
        /// and combines the resulting values into one observable sequence according to the combining selector,
        /// failing with <see cref="global::System.InvalidOperationException"/> if an element is empty.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>While the subscription to the source is active the sub-sequences are hot.</item>
        ///     <item>Multiple subscriptions and repeated subscriptions within the sub-sequences will not cause repeated subscriptions to the source.</item>
        ///     <item>Once the source sequence terminates it cannot be re-subscribed to with operators like <c>Repeat</c> or <c>Retry</c> from within a sub-sequence.</item>
        ///     <item>How termination (successful or error) of sub-sequences affects the resulting sequence depends on the combining operation.</item>
        /// </list>
        /// </remarks>
        /// <param name="source">An observable sequence whose elements to split into sub-sequences.</param>
        /// <param name="selector">Combine the individual sub-sequences into one resulting sequence.</param>
        /// <returns>An observable sequence that contains the elements of all sub-sequence.</returns>
        /// <returns>An observable sequence that contains the elements of all sub-sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            VisitMany<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::System.IObservable<global::BotMaster.Betterplace.MessageContract.Donation>, global::System.IObservable<TResult>> selector)
        {
            return global::System.Reactive.Linq.Observable.Create<TResult>(_o =>
            {
                var _mo = new VisitManyObserver(false);
                return global::System.Reactive.Disposables.StableCompositeDisposable.Create(
                    selector(_mo.Subject1).Subscribe(_o),
                    global::System.ObservableExtensions.SubscribeSafe(source, _mo),
                    _mo);
            });
        }

        /// <summary>
        /// Splits the observable sequence of BetterplaceMessage elements into one independent sub-sequences per value type,
        /// and combines the resulting values into one observable sequence according to the combining selector.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        ///     <item>While the subscription to the source is active the sub-sequences are hot.</item>
        ///     <item>Multiple subscriptions and repeated subscriptions within the sub-sequences will not cause repeated subscriptions to the source.</item>
        ///     <item>Once the source sequence terminates it cannot be re-subscribed to with operators like <c>Repeat</c> or <c>Retry</c> from within a sub-sequence.</item>
        ///     <item>How termination (successful or error) of sub-sequences affects the resulting sequence depends on the combining operation.</item>
        /// </list>
        /// </remarks>
        /// <param name="source">An observable sequence whose elements to split into sub-sequences.</param>
        /// <param name="selector">Combine the individual sub-sequences into one resulting sequence.</param>
        /// <returns>An observable sequence that contains the elements of all sub-sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            VisitMany<TResult>(
                this global::System.IObservable<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage> source,
                global::System.Func<global::System.IObservable<global::BotMaster.Betterplace.MessageContract.Donation>, global::System.IObservable<global::System.Reactive.Unit>, global::System.IObservable<TResult>> selector)
        {
            return global::System.Reactive.Linq.Observable.Create<TResult>(_o =>
            {
                var _mo = new VisitManyObserver(true);
                return global::System.Reactive.Disposables.StableCompositeDisposable.Create(
                    selector(_mo.Subject1, _mo.Subject0).Subscribe(_o),
                    global::System.ObservableExtensions.SubscribeSafe(source, _mo),
                    _mo);
            });
        }

        private sealed class VisitManyObserver
            : global::System.IObserver<global::BotMaster.Betterplace.MessageContract.BetterplaceMessage>, global::System.IDisposable
        {
            public readonly global::System.Reactive.Subjects.Subject<global::System.Reactive.Unit> Subject0 = new global::System.Reactive.Subjects.Subject<global::System.Reactive.Unit>();
            public readonly global::System.Reactive.Subjects.Subject<global::BotMaster.Betterplace.MessageContract.Donation> Subject1 = new global::System.Reactive.Subjects.Subject<global::BotMaster.Betterplace.MessageContract.Donation>();
            private readonly bool _accept0;

            public VisitManyObserver(bool _accept0)
            {
                this._accept0 = _accept0;
            }

            public void Dispose()
            {
                Subject1.Dispose();
                Subject0.Dispose();
            }

            public void OnNext(global::BotMaster.Betterplace.MessageContract.BetterplaceMessage _variant)
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant))
                {
                    case 0:
                        if (_accept0)
                        {
                            Subject0.OnNext(global::System.Reactive.Unit.Default);
                        }
                        else
                        {
                            OnError(global::dotVariant.GeneratorSupport.Errors.MakeEmptyError("BotMaster.Betterplace.MessageContract.BetterplaceMessage"));
                        }
                        break;
                    case 1:
                        Subject1.OnNext(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.Betterplace.MessageContract.Donation>)_variant).Value);
                        break;
                    default:
                        OnError(global::dotVariant.GeneratorSupport.Errors.MakeInternalError("BotMaster.Betterplace.MessageContract.BetterplaceMessage"));
                        break;
                }
            }

            public void OnError(global::System.Exception _ex)
            {
                Subject1.OnError(_ex);
                if (_accept0)
                {
                    Subject0.OnError(_ex);
                }
            }

            public void OnCompleted()
            {
                Subject1.OnCompleted();
                if (_accept0)
                {
                    Subject0.OnCompleted();
                }
            }
        }

    }
}
