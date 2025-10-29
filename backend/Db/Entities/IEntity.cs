namespace Db.Entities {
    public interface IEntity {
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    public interface IAvatarOwner {
        public Guid? AvatarId { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
