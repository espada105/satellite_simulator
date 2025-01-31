import subprocess
import sys
import os

def install_dependencies(venv_python):
    """필요한 모듈 설치 및 디버깅 정보 출력"""
    required_packages = ["opencv-python", "opencv-contrib-python", "numpy"]
    
    try:
        # 현재 설치된 패키지 목록 가져오기
        print("Checking installed packages...")
        result = subprocess.run(
            [venv_python, "-m", "pip", "list"],
            stdout=subprocess.PIPE,
            text=True,
            check=True
        )
        installed_packages = result.stdout
        print("Currently installed packages:")
        print(installed_packages)
        
        # 부족한 패키지 확인
        missing_packages = []
        for package in required_packages:
            if package not in installed_packages:
                missing_packages.append(package)
        
        if missing_packages:
            print(f"Missing packages detected: {missing_packages}")
        else:
            print("All required packages are already installed.")
        
        # pip 업그레이드
        print("Upgrading pip...")
        subprocess.run([venv_python, "-m", "pip", "install", "--upgrade", "pip"], check=True)
        
        # 필요한 패키지 설치
        if missing_packages:
            print(f"Installing missing packages: {missing_packages}")
            subprocess.run([venv_python, "-m", "pip", "install"] + missing_packages, check=True)
        
        print("Dependencies installed successfully.")
    except subprocess.CalledProcessError as e:
        print(f"Error occurred during installation: {e}")
        sys.exit(1)

def create_virtual_environment(venv_path):
    """가상환경 생성"""
    try:
        subprocess.run([sys.executable, "-m", "venv", venv_path], check=True)
        print(f"Virtual environment created at {venv_path}")
    except subprocess.CalledProcessError as e:
        print(f"Error occurred while creating virtual environment: {e}")
        sys.exit(1)

def activate_and_run():
    # 현재 스크립트가 있는 디렉토리를 기준으로 가상환경 경로 설정
    script_dir = os.path.dirname(os.path.abspath(__file__))  # 코드 위치
    venv_path = os.path.join(script_dir, ".venv")  # .venv는 Assets/PythonScripts/ 내에서 생성되도록 설정
    venv_python = os.path.join(venv_path, "Scripts", "python.exe") if os.name == "nt" else os.path.join(venv_path, "bin", "python")

    # 가상환경이 없으면 생성
    if not os.path.exists(venv_path):
        print("Virtual environment not found. Creating one...")
        create_virtual_environment(venv_path)

    # 의존성 설치
    install_dependencies(venv_python)

    # subprocess를 사용하여 가상환경 Python으로 현재 스크립트를 다시 실행
    subprocess.run([venv_python, os.path.abspath(__file__)], check=True)

if __name__ == "__main__":
    # 스크립트 경로 기준으로 가상환경 경로 설정
    script_dir = os.path.dirname(os.path.abspath(__file__))
    venv_path = os.path.join(script_dir, ".venv")

    if sys.prefix != venv_path:
        # 현재 Python이 가상환경이 아닌 경우, 가상환경으로 스크립트 실행
        activate_and_run()
    else:
        # 가상환경이 활성화된 상태에서 원래 작업 수행
        import cv2
        import json

        def feature_matching_with_sift(image1_path, image2_path, output_path):
            image1 = cv2.imread(image1_path, cv2.IMREAD_GRAYSCALE)
            image2 = cv2.imread(image2_path, cv2.IMREAD_GRAYSCALE)

            if image1 is None:
                raise FileNotFoundError(f"Error: Could not read image at {image1_path}")
            if image2 is None:
                raise FileNotFoundError(f"Error: Could not read image at {image2_path}")

            sift = cv2.SIFT_create()
            keypoints1, descriptors1 = sift.detectAndCompute(image1, None)
            keypoints2, descriptors2 = sift.detectAndCompute(image2, None)

            print(f"Keypoints in Image1: {len(keypoints1)}")
            print(f"Keypoints in Image2: {len(keypoints2)}")

            bf = cv2.BFMatcher()
            matches = bf.knnMatch(descriptors1, descriptors2, k=2)

            good_matches = []
            for m, n in matches:
                if m.distance < 0.75 * n.distance:
                    good_matches.append(m)

            total_keypoints = min(len(keypoints1), len(keypoints2))
            match_percentage = (len(good_matches) / total_keypoints) * 100 if total_keypoints > 0 else 0

            print(f"Number of good matches: {len(good_matches)}")
            print(f"Match Percentage: {match_percentage:.2f}%")

            result = cv2.drawMatches(image1, keypoints1, image2, keypoints2, good_matches, None, flags=cv2.DrawMatchesFlags_NOT_DRAW_SINGLE_POINTS)
            cv2.imwrite(output_image_path, result)

            # JSON 결과 저장
            result_data = {
                "keypoints1": len(keypoints1),
                "keypoints2": len(keypoints2),
                "good_matches": len(good_matches),
                "match_percentage": match_percentage
            }

            with open(output_path, "w") as f:
                json.dump(result_data, f)

            print(f"Results saved to {output_path}")

        # 이미지 경로 및 JSON 저장 경로를 상대 경로로 설정
        image1_path = os.path.join(script_dir, "Ralo.jpg")
        image2_path = os.path.join(script_dir, "Ralo.jpg")
        output_path = os.path.join(script_dir, "..", "Resources", "output.json")  # Resources 폴더에 json파일 저장
        output_image_path = os.path.join(script_dir, "..", "Resources", "output_image.jpg") # Resources 폴더에 이미지 저장

        feature_matching_with_sift(image1_path, image2_path, output_path)
