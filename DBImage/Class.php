<?php
	// 직업 이름을 받아와서 그 직업 관련된 모든 데이터를 반환해주는게 목표
	// 현재 사용하는 DB 이름은 ProjectDB
	// 테이블의 이름은 Class // 
	// column의 이름은 
	// Class
	// Class_Name // Class_WeaponUID // Class_StatusSkill0_UID // Class_StatusSkill1_UID // Class_StatusSkill2_UID // Class_AbilitySkill0_UID // Class_AbilitySkill1_UID
	// Class_Perk0_UID // Class_Perk1_UID // Class_Perk2_UID

	$className = $_POST["Input_className"];

	//$user = '박성훈';
	$className = iconv('utf-8', 'utf-8', $className);
	//euc-kr > utf-8
	
	// DB_IP 주소, 아이디, 패스워드
	$conn=mysqli_connect("localhost","root","1234");
		
	//if(mysqli_connect_errno($conn))
	//{
	//	echo "Fail to connect to MYSQL: " . msqli_connect_error();
	//}

	// 데이터 깨짐 방지
	mysqli_set_charset($conn,"utf8");
	mysqli_select_db($conn, "ProjectDB");

	//먼저 해당 아이디가 존재하는지 판단하기 위해서
	//SELECT 쿼리를 수행해줌
	$query = "SELECT * FROM Class WHERE Class_Name='$className'";

	//echo $query;


	// 결과를 받는 부분
	$res = mysqli_query($conn, $query);	
	//결과값의 갯수를 numrows에 저장
	//결과값이 존재하지 않으면 0을 반환
	//$numrows = mysqli_num_rows($res);    
	//echo $numrows;

	$rows = array();
	$result = array();


	while($row = mysqli_fetch_array($res))
	{
		$rows["ClassName"] = $row[0]; // Class_Name
		$rows["WeaponUID"] = $row[1]; // Class_WeaponUID
		$rows["StatusSkill0_UID"] = $row[2]; // Class_StatusSkill0_UID
		$rows["StatusSkill1_UID"] = $row[3]; // Class_StatusSkill1_UID
		$rows["StatusSkill2_UID"] = $row[4]; // Class_StatusSkill2_UID
		$rows["AbilitySkill0_UID"] = $row[5]; // Class_AbilitySkill0_UID
		$rows["AbilitySkill1_UID"] = $row[6]; // Class_AbilitySkill1_UID
		$rows["Perk0_UID"] = $row[7]; // Class_Perk0_UID
		$rows["Perk1_UID"] = $row[8]; // Class_Perk1_UID
		$rows["Perk2_UID"] = $row[9]; // Class_Perk2_UID

		array_push($result, $rows);

	}

	echo json_encode(array("results"=>$result));


	mysqli_close($conn);	
?>