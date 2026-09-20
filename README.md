# 🎮 2D Side-View Adventure Game Project

## 🚀 팀원 개발 환경 세팅 가이드 (최초 1회 실행)

본 프로젝트는 **.editorconfig** 및 **Husky.Net**을 활용하여 커밋 메시지 규격과 C# 코딩 컨벤션을 자동으로 검증합니다.  
프로젝트를 `clone` 또는 `pull` 받은 후 **최초 1회** 아래 세팅을 완료해 주세요.

---

### 1. 전제 조건 (Prerequisites)
- 컴퓨터에 **.NET SDK**가 설치되어 있어야 합니다. (`dotnet --version`으로 확인)
    - 미설치 시: [.NET SDK 다운로드](https://dotnet.microsoft.com/download) 후 설치 및 IDE/터미널 재시작

---

### 2. 세팅 방법 (택 1)

#### Option A. 자동 세팅 (권장 - Windows)
프로젝트 루트 폴더에 있는 **`setup.bat`** 파일을 더블 클릭하여 실행합니다.

#### Option B. 수동 세팅 (Rider Terminal / Git Bash)
Rider 하단 **Terminal** (`Alt + F12`)을 열고 아래 명령어 두 줄을 실행합니다:
```bash
dotnet tool restore
dotnet husky install