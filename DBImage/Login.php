<?php

	// Unity에서 전달받은 값을 변수에 할당하는 부분.
	$user = $_POST["Input_user"];
	$pass = $_POST["Input_pass"];

	// Test
	$user = '55555';
	//$user = '박성훈';
	//$user = iconv('euc-kr', 'utf-8', $user);
	//euc-kr > utf-8
	// Encoding 변경하는 부분
	$user = iconv('utf-8', 'utf-8', $user);
	
	// DB에 접속하는 부분(DB위치주소, 아이디, 패스워드)
	$conn=mysqli_connect("localhost","root","1234");
	
	// 현재 에러 발생 부분.
	//if(mysqli_connect_errno($conn))
	//{
	//	echo "Fail to connect to MYSQL: " . msqli_connect_error();
	//}
	// DBMS에 요청하는 것.
	// 데이터 인코딩 깨짐 방지
	mysqli_set_charset($conn,"utf8");
	
	// 내가 접속하고자 하는 데이터베이스(DB) 선택
	mysqli_select_db($conn, "study");

	// 쿼리에서 회원 아이디로 회원이 존재하는지 확인
	$query = "SELECT * FROM User WHERE userId='$user'";
	//$query = "SELECT * FROM User";
	// SELECT * FROM User WHERE userId='123';

	//echo $query;
	
	// 이 부분이 연결된 DB에 SQL을 날린 후 결과를 받는 부분
	$res = mysqli_query($conn, $query);	
	
	$rows = array();
	$result = array();


	while($row = mysqli_fetch_array($res))
	{
		// []안의 이름은 원하는 방식으로 작성 가능
		// 실제 Column 명과 표시용 Column명은 다를 수 있다.
		$rows["ID"] = $row[0];//id
		$rows["PASS"] = $row[1];//password
		$rows["NOTE"]= $row[2];//note
		array_push($result, $rows);
		//php에서 echo는 printf 와 같은 기능을한다.
		//출력 결과물을 화면에 표시해준다
		//echo $row[0];
	}
	echo json_encode(array("results"=>$result));
	
	mysqli_close($conn);	

?>