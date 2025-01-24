import subprocess
import sys
import os

def activate_and_run():
    # 현재 스크립트가 있는 디렉토리를 기준으로 가상환경 경로 설정
    script_dir = os.path.dirname(os.path.abspath(__file__))
    venv_python = os.path.join(script_dir, ".venv", "Scripts", "python.exe")

    # 스크립트 경로 설정
    script_path = os.path.abspath(__file__)

    # subprocess를 사용하여 가상환경 Python으로 현재 스크립트를 다시 실행
    subprocess.run([venv_python, script_path], check=True)

if __name__ == "__main__":
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
            cv2.imshow("SIFT Feature Matching", result)
            cv2.waitKey(0)
            cv2.destroyAllWindows()

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
        output_path = os.path.join(script_dir, "..", "Resources", "output.json")  # Resources 폴더에 저장

        feature_matching_with_sift(image1_path, image2_path, output_path)
