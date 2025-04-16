# 🤝 JHYProject 개발 규칙 (Contribution Guide)

혼자 작업하는 Unity 프로젝트라도 커밋, 브랜치, 코드 작성 규칙을 정리해두면  
이후 협업이나 리팩토링 시 많은 도움이 됩니다.

---

## 🧾 커밋 메시지 규칙 (Commit Message Convention)

**형식**: <type>(scope): 메시지 요약

**타입 목록**:

| 타입         | 의미            | 예시                       |
|------------|---------------|--------------------------|
| `feat`     | 새로운 기능 추가     | `feat(player): 점프 추가`    |
| `fix`      | 버그 수정         | `fix(ui): 팝업 안 닫힘 수정`    |
| `refactor` | 리팩토링          | `refactor(scene): 구조 정리` |
| `style`    | 포맷팅/네이밍 정리    | `style: 변수명 통일`          |
| `chore`    | 설정 파일, 패키지 작업 | `chore: .gitignore 업데이트` |
| `docs`     | 문서 변경         | `docs: README 작성`        |
| `test`     | 테스트 코드 작성     | `test: Player 테스트 추가`    |
| `perf`     | 성능 개선         | `perf: Update 루프 최적화`    |
| `asset`    | 에셋 추가         | asset: quiky 에셋 추가`      |

---

## 🌿 브랜치 네이밍 규칙 (Branch Naming)

**형식**: `<type>/<짧은-설명>`

| 타입 | 예시 |
|------|------|
| `feature/` | `feature/combo-system` |
| `fix/` | `fix/ui-popup` |
| `refactor/` | `refactor/player-control` |
| `chore/` | `chore/add-dotween` |

✅ 소문자와 `-`(하이픈) 사용  
✅ 기능/수정 단위로 분리

---

## 🧱 코드 컨벤션 (Coding Convention)

**C# 스타일 규칙**

- 클래스명: `PascalCase`
- 메서드명: `PascalCase`
- 변수명: `camelCase`
- private 변수: `_camelCase` (언더스코어 붙임)
- `SerializedField`: 항상 `private` + `[SerializeField]`
- 파일명: 클래스명과 동일

**Unity 특화 규칙**

- `Start`, `Update` 등도 `private` 등 명시적으로 선언
- 씬 루트 GameObject 이름은 `@Player` 같은 접두어 사용 가능

---

## 🔀 Git 작업 흐름 (Git Workflow)

- 모든 작업은 `main`에서 파생된 브랜치에서 수행
- 커밋은 단일 목적에 맞게 작고 명확하게
- (옵션) 완료된 브랜치는 squash merge 또는 rebase로 정리

---

> 규칙은 프로젝트 진행에 따라 유동적으로 개선될 수 있습니다.
