#nullable disable
namespace BotMaster.MessageContract
{
    [global::System.Diagnostics.DebuggerTypeProxy(typeof(DefinedMessage.__DebuggerTypeProxy))]
    [global::System.Diagnostics.DebuggerDisplay("{_variant.AsObject}", Type = "{_variant.TypeString,nq}")]
    partial class DefinedMessage
        : global::System.IEquatable<DefinedMessage>
    {
        private readonly global::dotVariant._G.BotMaster.MessageContract.DefinedMessage _variant;

        /// <summary>
        /// Create a DefinedMessage with a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.
        /// </summary>
        /// <param name="textMessage">The value to initlaize the variant with.</param>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public DefinedMessage(global::BotMaster.MessageContract.TextMessage textMessage)
            => _variant = new global::dotVariant._G.BotMaster.MessageContract.DefinedMessage(textMessage);

        /// <summary>
        /// Create a DefinedMessage with a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.
        /// </summary>
        /// <param name="textMessage">The value to initlaize the variant with.</param>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static implicit operator DefinedMessage(global::BotMaster.MessageContract.TextMessage textMessage)
            => new DefinedMessage(textMessage);

        /// <summary>
        /// Create a DefinedMessage with a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.
        /// </summary>
        /// <param name="textMessage">The value to initlaize the variant with.</param>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static DefinedMessage Create(global::BotMaster.MessageContract.TextMessage textMessage)
            => new DefinedMessage(textMessage);


        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.IsEmpty"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool IsEmpty
            => _variant.IsEmpty;

        /// <inheritdoc/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public override bool Equals(object other)
            => other is DefinedMessage v
            && Equals(v);

        /// <inheritdoc/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool Equals(DefinedMessage other)
            => !(other is null) && _variant.Equals(other._variant);

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static bool operator ==(DefinedMessage lhs, DefinedMessage rhs)
            => lhs?.Equals(rhs) ?? (rhs is null);

        [global::System.Diagnostics.DebuggerNonUserCode]
        public static bool operator !=(DefinedMessage lhs, DefinedMessage rhs)
            => !(lhs == rhs);

        [global::System.Diagnostics.DebuggerNonUserCode]
        public override int GetHashCode()
            => _variant.GetHashCode();

        [global::System.Diagnostics.DebuggerNonUserCode]
        public override string ToString()
            => _variant.ToString();

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.TryMatch(out global::BotMaster.MessageContract.TextMessage)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool TryMatch(out global::BotMaster.MessageContract.TextMessage textMessage)
            => _variant.TryMatch(out textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.TryMatch(global::System.Action{global::BotMaster.MessageContract.TextMessage})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public bool TryMatch(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage)
            => _variant.TryMatch(textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Match(out global::BotMaster.MessageContract.TextMessage)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Match(out global::BotMaster.MessageContract.TextMessage textMessage)
            => _variant.Match(out textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Match(global::System.Action{global::BotMaster.MessageContract.TextMessage})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Match(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage)
            => _variant.Match(textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Match(global::System.Action{global::BotMaster.MessageContract.TextMessage}, global::System.Action)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Match(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage, global::System.Action _)
            => _variant.Match(textMessage, _);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Match{TResult}(global::System.Func{global::BotMaster.MessageContract.TextMessage, TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Match<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
            => _variant.Match(textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Match{TResult}(global::System.Func{global::BotMaster.MessageContract.TextMessage, TResult}, TResult)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Match<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage, TResult _)
            => _variant.Match(textMessage, _);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Match{TResult}(global::System.Func{global::BotMaster.MessageContract.TextMessage, TResult}, global::System.Func{TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Match<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage, global::System.Func<TResult> _)
            => _variant.Match(textMessage, _);


        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Visit(global::System.Action{global::BotMaster.MessageContract.TextMessage})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Visit(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage)
            => _variant.Visit(textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Visit(global::System.Action{global::BotMaster.MessageContract.TextMessage}, global::System.Action)"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public void Visit(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage, global::System.Action _)
            => _variant.Visit(textMessage, _);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Visit{TResult}(global::System.Func{global::BotMaster.MessageContract.TextMessage, TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
            => _variant.Visit(textMessage);

        /// <inheritdoc cref="global::dotVariant._G.BotMaster.MessageContract.DefinedMessage.Visit{TResult}(global::System.Func{global::BotMaster.MessageContract.TextMessage, TResult}, global::System.Func{TResult})"/>
        [global::System.Diagnostics.DebuggerNonUserCode]
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage, global::System.Func<TResult> _)
            => _variant.Visit(textMessage, _);

        private sealed class __DebuggerTypeProxy
        {
            public object Value { get; }
            public __DebuggerTypeProxy(DefinedMessage v)
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
        public static explicit operator global::dotVariant.GeneratorSupport.Discriminator(DefinedMessage v)
            => (global::dotVariant.GeneratorSupport.Discriminator)v._variant;
        [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
        [global::System.Diagnostics.DebuggerNonUserCode]
        public static explicit operator global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>(DefinedMessage v)
            => (global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)v._variant;
    }
}

namespace dotVariant._G.BotMaster.MessageContract
{
    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.Diagnostics.DebuggerDisplay("{AsObject}", Type = "{TypeString,nq}")]
    internal readonly struct DefinedMessage
    {
        private readonly struct Union
        {
            public readonly global::BotMaster.MessageContract.TextMessage _1;
            public Union(global::BotMaster.MessageContract.TextMessage value)
            {
                _1 = value;
            }
        }

        private readonly Union _x;
        private readonly byte _n;

        public DefinedMessage(global::BotMaster.MessageContract.TextMessage textMessage)
        {
            _n = 1;
            _x = new Union(textMessage);
        }


        public static explicit operator global::dotVariant.GeneratorSupport.Discriminator(in DefinedMessage v)
            => (global::dotVariant.GeneratorSupport.Discriminator)v._n;
        public static explicit operator global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>(in DefinedMessage v)
            => new global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>(v._x._1);

        /// <summary>
        /// <see langword="true"/> if DefinedMessage was constructed without a value.
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
                        return "BotMaster.MessageContract.TextMessage";
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<string>("BotMaster.MessageContract.DefinedMessage");
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
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<string>("BotMaster.MessageContract.DefinedMessage");
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
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<object>("BotMaster.MessageContract.DefinedMessage");
                }
            }
        }

        public bool Equals(in DefinedMessage other)
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
                    return global::System.Collections.Generic.EqualityComparer<global::BotMaster.MessageContract.TextMessage>.Default.Equals(_x._1, other._x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<bool>("BotMaster.MessageContract.DefinedMessage");
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
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<int>("BotMaster.MessageContract.DefinedMessage");
            }
        }

        /// <summary>
        /// Retrieve the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.
        /// </summary>
        /// <param name="textMessage">Receives the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <returns><see langword="true"/> if DefinedMessage contained a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</returns>
        public bool TryMatch(out global::BotMaster.MessageContract.TextMessage textMessage)
        {
            textMessage = _n == 1 ? _x._1 : default;
            return _n == 1;
        }

        /// <summary>
        /// Invoke a delegate with the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <returns><see langword="true"/> if DefinedMessage contained a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="textMessage"> is rethrown.</exception>
        public bool TryMatch(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage)
        {
            if (_n == 1)
            {
                textMessage(_x._1);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieve the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>,
        /// otherwise throw <see cref="global::System.InvalidOperationException"/>.
        /// </summary>
        /// <param name="textMessage">Receives the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">DefinedMessage does not contain a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</exception>
        public void Match(out global::BotMaster.MessageContract.TextMessage textMessage)
        {
            if (_n == 1)
            {
                textMessage = _x._1;
                return;
            }
            throw global::dotVariant.GeneratorSupport.Errors.MakeMismatchError("BotMaster.MessageContract.DefinedMessage", "BotMaster.MessageContract.TextMessage", TypeString);
        }

        /// <summary>
        /// Invoke a delegate with the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>,
        /// otherwise throw <see cref="global::System.InvalidOperationException"/>.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">DefinedMessage does not contain a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="textMessage"/> is rethrown.</exception>
        public void Match(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage)
        {
            if (_n == 1)
            {
                textMessage(_x._1);
                return;
            }
            global::dotVariant.GeneratorSupport.Errors.ThrowMismatchError("BotMaster.MessageContract.DefinedMessage", "BotMaster.MessageContract.TextMessage", TypeString);
        }

        /// <summary>
        /// Invoke a delegate with the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>,
        /// otherwise invoke an alternative delegate.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The delegate to invoke if the stored value is of a different type.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="textMessage"/> or <paramref name="_"/> is rethrown.</exception>
        public void Match(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage, global::System.Action _)
        {
            if (_n == 1)
            {
                textMessage(_x._1);
            }
            else
            {
                _();
            }
        }

        /// <summary>
        /// Invoke a delegate with the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/> and return the result,
        /// otherwise throw <see cref="global::System.InvalidOperationException"/>.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <returns>The value returned from invoking <paramref name="textMessage"/>.</returns>
        /// <exception cref="global::System.InvalidOperationException">DefinedMessage does not contain a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="textMessage"/> is rethrown.</exception>
        public TResult Match<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
        {
            if (_n == 1)
            {
                return textMessage(_x._1);
            }
            return global::dotVariant.GeneratorSupport.Errors.ThrowMismatchError<TResult>("BotMaster.MessageContract.DefinedMessage", "BotMaster.MessageContract.TextMessage", TypeString);
        }

        /// <summary>
        /// Invoke a delegate with the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/> and return the result,
        /// otherwise return a provided value.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The value to return if the stored value is of a different type.</param>
        /// <returns>The value returned from invoking <paramref name="textMessage"/>, or <paramref name="default"/>.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="textMessage"/> or <paramref name="_"/> is rethrown.</exception>
        public TResult Match<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage, TResult _)
        {
            return _n == 1 ? textMessage(_x._1) : _;
        }

        /// <summary>
        /// Invoke a delegate with the value stored within DefinedMessage if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/> and return the result,
        /// otherwise invoke an alternative delegate and return its result.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke with the stored value if it is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The delegate to invoke if the stored value is of a different type.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from <paramref name="textMessage"/> or <paramref name="_"/> is rethrown.</exception>
        public TResult Match<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage, global::System.Func<TResult> _)
        {
            return _n == 1 ? textMessage(_x._1) : _();
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of type of the value stored within DefinedMessage,
        /// and invoke a special delegate if DefinedMessage is empty.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The delegate to invoke if DefinedMessage is empty.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        public void Visit(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage, global::System.Action _)
        {
            switch (_n)
            {
                case 0:
                    _();
                    break;
                case 1:
                    textMessage(_x._1);
                    break;
                default:
                    global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.MessageContract.DefinedMessage");
                    break;
            }
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of the value stored within DefinedMessage,
        /// and throw an exception if DefinedMessage is empty.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">DefinedMessage is empty.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        public void Visit(global::System.Action<global::BotMaster.MessageContract.TextMessage> textMessage)
        {
            switch (_n)
            {
                case 0:
                    global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError("BotMaster.MessageContract.DefinedMessage");
                    break;
                case 1:
                    textMessage(_x._1);
                    break;
                default:
                    global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.MessageContract.DefinedMessage");
                    break;
            }
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of type of the value stored within DefinedMessage and return the result,
        /// and invoke a special delegate if DefinedMessage is empty and return its result.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The delegate to invoke if DefinedMessage is empty.</param>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The return type of all delegates, and by extension the return type of this function.</typeparam>
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage, global::System.Func<TResult> _)
        {
            switch (_n)
            {
                case 0:
                    return _();
                case 1:
                    return textMessage(_x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.MessageContract.DefinedMessage");
            }
        }

        /// <summary>
        /// Invoke the delegate whose parameter type matches that of the value stored within DefinedMessage and return the result,
        /// and throw an exception if DefinedMessage is empty.
        /// </summary>
        /// <param name="textMessage">The delegate to invoke if the stored value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <exception cref="global::System.InvalidOperationException">DefinedMessage is empty.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The return type of all delegates, and by extension the return type of this function.</typeparam>
        public TResult Visit<TResult>(global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
        {
            switch (_n)
            {
                case 0:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError<TResult>("BotMaster.MessageContract.DefinedMessage");
                case 1:
                    return textMessage(_x._1);
                default:
                    return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.MessageContract.DefinedMessage");
            }
        }
    }
}


namespace BotMaster.MessageContract
{
    public static partial class DefinedMessageEx
    {
        /// <summary>
        /// Transform a DefinedMessage-based enumerable sequence by applying a selector function to those elements
        /// containing a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/> and dropping all others.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="textMessage">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Match<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
        {
            foreach (var variant in source)
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant) == 1)
                {
                    yield return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)variant).Value);
                }
            }
        }

        /// <summary>
        /// Transform a DefinedMessage-based enumerable sequence by applying a selector function to those elements
        /// containing a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/> and replacing all others by a fallback value.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="textMessage">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Match<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage,
                TResult _)
        {
            foreach (var variant in source)
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant) == 1)
                {
                    yield return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)variant).Value);
                }
                else
                {
                    yield return _;
                }
            }
        }

        /// <summary>
        /// Transform a DefinedMessage-based enumerable sequence by applying a selector function to those elements
        /// containing a value of type <see cref="global::BotMaster.MessageContract.TextMessage"/> and replacing all others with the result of a fallback selector.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="textMessage">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Match<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage,
                global::System.Func<TResult> _)
        {
            foreach (var variant in source)
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant) == 1)
                {
                    yield return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)variant).Value);
                }
                else
                {
                    yield return _();
                }
            }
        }

        /// <summary>
        /// Transform a DefinedMessage-based enumerable sequence by applying a selector function to each element
        /// wich matches the type stored within the value, and throwing <see cref="global::System.InvalidOperationException"/>
        /// if an element is empty.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="textMessage">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.InvalidOperationException">The sequence encountered an empty DefinedMessage.</exception>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Visit<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
        {
            foreach (var variant in source)
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)variant))
                {
                    case 0:
                        global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError("BotMaster.MessageContract.DefinedMessage");
                        yield break;
                    case 1:
                        yield return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)variant).Value);
                        break;
                    default:
                        global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.MessageContract.DefinedMessage");
                        yield break;
                }
            }
        }

        /// <summary>
        /// Transform a DefinedMessage-based enumerable sequence by applying a selector function to each element
        /// wich matches the type stored within the value, and replacing empty elements with the result of a fallback selector.
        /// </summary>
        /// <param name="source">An enumerable sequence whose elements to match on.</param>
        /// <param name="textMessage">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The delegate to invoke if an element is empty.</param>
        /// <returns>An enumerable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <exception cref="global::System.Exception">Any exception thrown from a delegate is rethrown.</exception>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.Collections.Generic.IEnumerable<TResult>
            Visit<TResult>(
                this global::System.Collections.Generic.IEnumerable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage,
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
                        yield return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)variant).Value);
                        break;
                    default:
                        global::dotVariant.GeneratorSupport.Errors.ThrowInternalError("BotMaster.MessageContract.DefinedMessage");
                        yield break;
                }
            }
        }
    }
}
namespace BotMaster.MessageContract
{
    public static partial class DefinedMessageEx
    {
        /// <summary>
        /// Projects each <see cref="global::BotMaster.MessageContract.TextMessage"/> element of an observable sequence
        /// into a new form and drops all other elements.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to match on.</param>
        /// <param name="textMessage">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <returns>An observable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Match<TResult>(
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
        {
            return global::System.Reactive.Linq.Observable.Select(
                global::System.Reactive.Linq.Observable.Where(source, _variant => ((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant) == 1),
                _variant => textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)_variant).Value));
        }

        /// <summary>
        /// Projects each <see cref="global::BotMaster.MessageContract.TextMessage"/> element of an observable sequence
        /// into a new form and replaces all other elements by a fallback value.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to match on.</param>
        /// <param name="textMessage">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An observable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Match<TResult>(
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage,
                TResult _)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant) == 1)
                {
                    return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)_variant).Value);
                }
                else
                {
                    return _;
                }
            });
        }

        /// <summary>
        /// Projects each <see cref="global::BotMaster.MessageContract.TextMessage"/> element of an observable sequence
        /// into a new form and replaces all other elements by a fallback selector result.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to match on.</param>
        /// <param name="textMessage">Function applied to matching elements and whose value to surface from the resulting sequence.</param>
        /// <param name="_">Value to produce for elements which do not match the desired type.</param>
        /// <returns>An observable sequence that contains the matched and transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Match<TResult>(
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage,
                global::System.Func<TResult> _)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                if (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant) == 1)
                {
                    return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)_variant).Value);
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
        /// <param name="textMessage">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <returns>An observable sequence that contains the transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Visit<TResult>(
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant))
                {
                    case 0:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowEmptyError<TResult>("BotMaster.MessageContract.DefinedMessage");
                    case 1:
                        return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)_variant).Value);
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.MessageContract.DefinedMessage");
                }
            });
        }

        /// <summary>
        /// Projects each element of an observable sequence into a new form depending on its contained value type,
        /// failing with <see cref="global::System.InvalidOperationException"/> if an element is empty.
        /// </summary>
        /// <param name="source">An observable sequence whose elements to visit.</param>
        /// <param name="textMessage">The delegate to invoke if the element's value is of type <see cref="global::BotMaster.MessageContract.TextMessage"/>.</param>
        /// <param name="_">The delegate to invoke if an element is empty.</param>
        /// <returns>An observable sequence that contains the transformed elements of the input sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            Visit<TResult>(
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::BotMaster.MessageContract.TextMessage, TResult> textMessage,
                global::System.Func<TResult> _)
        {
            return global::System.Reactive.Linq.Observable.Select(source, _variant =>
            {
                switch (((int)(global::dotVariant.GeneratorSupport.Discriminator)_variant))
                {
                    case 0:
                        return _();
                    case 1:
                        return textMessage(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)_variant).Value);
                    default:
                        return global::dotVariant.GeneratorSupport.Errors.ThrowInternalError<TResult>("BotMaster.MessageContract.DefinedMessage");
                }
            });
        }


        /// <summary>
        /// Splits the observable sequence of DefinedMessage elements into one independent sub-sequences per value type,
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
        /// <param name="textMessage">Transform an observable sequence of <see cref="global::BotMaster.MessageContract.TextMessage"/> values into an observable sequence of <typeparamref name="TResult"/> values.</param>
        /// <param name="_">Transform a sequence of <see cref="global::System.Reactive.Unit"/> values (each representing an empty variant) into a sequence of <typeparamref name="TResult"/> values.</param>
        /// <returns>An observable sequence that contains the elements of all sub-sequence.</returns>
        /// <typeparam name="TResult">The resulting sequence's element type.</typeparam>
        public static global::System.IObservable<TResult>
            VisitMany<TResult>(
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::System.IObservable<global::BotMaster.MessageContract.TextMessage>, global::System.IObservable<TResult>> textMessage,
                global::System.Func<global::System.IObservable<global::System.Reactive.Unit>, global::System.IObservable<TResult>> _)
        {
            return VisitMany(source, (_1, _0) =>
            {
                return global::System.Reactive.Linq.Observable.Merge(textMessage(_1), _(_0));
            });
        }

        /// <summary>
        /// Splits the observable sequence of DefinedMessage elements into one independent sub-sequences per value type,
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
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::System.IObservable<global::BotMaster.MessageContract.TextMessage>, global::System.IObservable<TResult>> selector)
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
        /// Splits the observable sequence of DefinedMessage elements into one independent sub-sequences per value type,
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
                this global::System.IObservable<global::BotMaster.MessageContract.DefinedMessage> source,
                global::System.Func<global::System.IObservable<global::BotMaster.MessageContract.TextMessage>, global::System.IObservable<global::System.Reactive.Unit>, global::System.IObservable<TResult>> selector)
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
            : global::System.IObserver<global::BotMaster.MessageContract.DefinedMessage>, global::System.IDisposable
        {
            public readonly global::System.Reactive.Subjects.Subject<global::System.Reactive.Unit> Subject0 = new global::System.Reactive.Subjects.Subject<global::System.Reactive.Unit>();
            public readonly global::System.Reactive.Subjects.Subject<global::BotMaster.MessageContract.TextMessage> Subject1 = new global::System.Reactive.Subjects.Subject<global::BotMaster.MessageContract.TextMessage>();
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

            public void OnNext(global::BotMaster.MessageContract.DefinedMessage _variant)
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
                            OnError(global::dotVariant.GeneratorSupport.Errors.MakeEmptyError("BotMaster.MessageContract.DefinedMessage"));
                        }
                        break;
                    case 1:
                        Subject1.OnNext(((global::dotVariant.GeneratorSupport.Accessor_1<global::BotMaster.MessageContract.TextMessage>)_variant).Value);
                        break;
                    default:
                        OnError(global::dotVariant.GeneratorSupport.Errors.MakeInternalError("BotMaster.MessageContract.DefinedMessage"));
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
