using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotMaster.Database
{
    public abstract class IdEntity<T> : Entity, IEquatable<IdEntity<T>?>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T Id { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as IdEntity<T>);
        }

        public bool Equals(IdEntity<T>? other)
        {
            return other is not null &&
                   EqualityComparer<T>.Default.Equals(Id, other.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(IdEntity<T>? left, IdEntity<T>? right)
        {
            return EqualityComparer<IdEntity<T>>.Default.Equals(left, right);
        }

        public static bool operator !=(IdEntity<T>? left, IdEntity<T>? right)
        {
            return !(left == right);
        }
    }
}
