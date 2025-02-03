# SATELLITE SIMULATOR
#### Unity, Google Maps Statics API, OpenCV를 활용한 인공위성 카메라 시뮬레이터

### TEAM
<table>
  <tr align="center">
    <td width="150px">
      <a href="https://github.com/gugyeoj1n" target="_blank">
        <img src="https://avatars.githubusercontent.com/gugyeoj1n" alt="gugyeoj1n" />
      </a>
    </td>
    <td width="150px">
      <a href="https://github.com/espada105" target="_blank">
        <img src="https://avatars.githubusercontent.com/espada105" alt="espada105" />
      </a>
    </td>
  </tr>

  <tr align="center">
    <td>
      곽우진
    </td>
    <td>
      홍성인
    </td>
  </tr>

  <tr align="center">
    <td>
      UI<br>GMS API
    </td>
    <td>
      위성 구현<br>OpenCV
    </td>
  </tr>  
  
</table>

### 사용 기술
    Unity 6000.0.32f1
    Google Maps Statics API
    OpenCV SIFT Feature Matching

### 설명

    지구 주위를 일정한 궤도로 도는 위성을 씬 상에 구현해 두었습니다.
    위성 카메라에서 RayCast를 발사한 지점의 좌표를 실제 위도, 경도로 변환하여 Google Maps Statics API를 호출해 해당 지점의 사진을 수신합니다.
    사용자는 특정 사진을 업로드해 위성이 촬영한 사진과 비교 작업을 수행할 수 있습니다.
    내장된 OpenCV 모델이 SIFT 알고리즘을 기반으로 두 이미지의 특징을 비교한 후, 결과값을 도출합니다.

    프로그램 실행 시 사용자 컴퓨터에 파이썬이 설치되어 있는지 확인하는 과정을 거치고, 가상환경과 모듈 역시 자동으로 설치됩니다.


### 스크린샷
<img src="https://github.com/user-attachments/assets/5bf6fb5a-e33b-4062-a28d-77b971e6aef9" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/314e3ab5-fb80-4b19-8521-b05721346e46" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/3d163aa2-6290-456f-85fb-0b271f8b9bfe" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/b3b6b334-3d62-43b8-8be8-dc3aeae53351" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/5570c9b7-057a-4961-8000-6d785590662e" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/48dd438c-381f-4f6b-b34b-89d111a3002d" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/9c8258d2-16c3-4434-911e-b0d6d176a41c" width="450" height="250"/>
<img src="https://github.com/user-attachments/assets/f1b5c82e-6a70-4052-a36e-3d2b84607bef" width="450" height="250"/>
