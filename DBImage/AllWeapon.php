<?php
	// 무기 UID 값을 받아와서 그 무기와 관련된 모든 데이터를 반환해주는게 목표
	// 현재 사용하는 DB 이름은 ProjectDB
	// 테이블의 이름은 Weapon // Class // 
	// column의 이름은 
	// Weapon
	// Weapon_UID // Weapon_Name // Weapon_Damage // Weapon_AttackSpeed // Weapon_AttackDistance // Weapon_ReloadBullet // Weapon_CarryBullet // Weapon_ReloadTime
	// Weapon_AttackRange
	// 모든 무기의 정보를 받을 것이라 입력 값은 없다.
	//$weapon_UID = $_POST["Input_weaponUID"];

	//error_reporting(E_ALL);
    //ini_set("display_errors", 1);
	
	// DB 서버 접속
	$conn=mysqli_connect("localhost","root","1234");
		
	//if(mysqli_connect_errno($conn))
	//{
	//	echo "Fail to connect to MYSQL: " . msqli_connect_error();
	//}

	// 데이터 깨짐 방지
	mysqli_set_charset($conn,"utf8");
	// DB 선택
	mysqli_select_db($conn, "ProjectDB");

	//먼저 해당 아이디가 존재하는지 판단하기 위해서
	//SELECT 쿼리를 수행해줌
	// 모든 Weapon 선택
	$query = "SELECT * FROM Weapon";

	//echo $query;

	$res = mysqli_query($conn, $query);	
	//결과값의 갯수를 numrows에 저장
	//결과값이 존재하지 않으면 0을 반환
	$numrows = mysqli_num_rows($res);    

	//echo $numrows;

	$rows = array();
	$result = array();

	while($row = mysqli_fetch_array($res)){
		$rows["Weapon_UID"] = $row[0]; // Weapon_UID
		$rows["Weapon_Name"] = $row[1]; // Weapon_Name
		$rows["Weapon_Damage"] = $row[2]; // Weapon_Damage;
		$rows["Weapon_AttackSpeed"] = $row[3]; // Weapon_AttackSpeed;
		$rows["Weapon_AttackDistance"] = $row[4]; // Weapon_AttackDistance
		$rows["Weapon_ReloadBullet"] = $row[5]; // Weapon_ReloadBullet
		$rows["Weapon_CarryBullet"] = $row[6]; // Weapon_CarryBullet
		$rows["Weapon_ReloadTime"] = $row[7]; // Weapon_ReloadTime
		$rows["Weapon_AttackRange"] = $row[8]; // Weapon_AttackRange

		array_push($result, $rows);
	}

	echo json_encode(array("results"=>$result));

	mysqli_close($conn);	
?>