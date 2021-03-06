using UnityEngine;

public static class EConfig {
    public static class Map {
        public static readonly float MIN_DISTANCE = 2.5f;
        public static readonly float MAX_DISTANCE = 4f;
        public static readonly float MIN_SIZE = 1f;
        public static readonly float MAX_SIZE = 2f;
    }

    public static class System {
        public static readonly float MOVE_SPEED = 8f;
        public static readonly float CORRECTION_VALUE = 0.5f;
        public static readonly float DEFAULT_CANVAS_WIDTH = 720f;
        public static readonly float DEFAULT_CANVAS_HEIGHT = 1280f;
        public static readonly Vector3 DIRECTION_RIGHT = new Vector3(1, 0, 1);
        public static readonly Vector3 DIRECTION_LEFT = new Vector3(-1, 0, 1);
    }

    public static class Character {
        public static readonly ECharacterType INITIAL_CHARACTER = ECharacterType.Chick;
    }

    public enum ELogLevel {
        None,
        Error,
        Warning,
        All,
    }

    public enum ELanguage {
        ENGLISH,
        KOREAN
    }
}
