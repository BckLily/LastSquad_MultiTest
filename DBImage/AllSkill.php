<?php
	// 직업 이름을 받아와서 그 직업 관련된 모든 데이터를 반환해주는게 목표
	// 현재 사용하는 DB 이름은 ProjectDB
	// 테이블의 이름은 Class // 
	// column의 이름은 
	// Skill
	// Skill_UID // Skill_Name
	
	
	// DB_IP 주소, 아이디, 패스워드
	$conn=mysqli_connect("localhost","root","1234");
		
	//if(mysqli_connect_errno($conn))
	//{
	//	echo "Fail to connect to MYSQL: " . msqli_connect_error();
	//}

	// 데이터 깨짐 방지
	mysqli_set_charset($conn,"utf8");
	mysqli_select_db($conn, "ProjectDB");

	
	//SELECT 쿼리를 수행해줌
	$query = "SELECT * FROM Skill";

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
		$rows["Skill_UID"] = $row[0]; // Skill_UID
		$rows["Skill_Name"] = $row[1]; // Skill_Name

		array_push($result, $rows);

	}

	echo json_encode(array("results"=>$result));

	mysqli_close($conn);
?>