<?php

	$user = $_POST["Input_user"];
	$pass = $_POST["Input_pass"];
	$memo = $_POST["Input_Info"];

//	error_reporting(E_ALL);
//    ini_set("display_errors", 1);

	//$user = '박성훈';
	$user = iconv('utf-8', 'utf-8', $user);
	//euc-kr > utf-8
	//$user = iconv('utf-8', 'utf-8', $user);
	//$memo 도 한국어가 들어올 수 있으므로, 인코딩 문제를 해결해줌
	$memo = iconv('utf-8', 'utf-8', $memo);
	
	$conn=mysqli_connect("localhost","root","1234");
		
	//if(mysqli_connect_errno($conn))
	//{
	//	echo "Fail to connect to MYSQL: " . msqli_connect_error();
	//}
	mysqli_set_charset($conn,"utf8");
	
	mysqli_select_db($conn, "study");

	//먼저 해당 아이디가 존재하는지 판단하기 위해서
	//SELECT 쿼리를 수행해줌
	// 문자열 결합 "asdf"."qwert"
	$query = "SELECT * FROM User WHERE userId='$user'";

	//echo $query;

	$res = mysqli_query($conn, $query);	
	//결과값의 갯수를 numrows에 저장
	//결과값이 존재하지 않으면 0을 반환
	$numrows = mysqli_num_rows($res);    

	//echo $numrows;

	if($numrows == 0)
	{
		//INSERT INTO `user`(`id`, `password`, `note`) VALUES ([value-1],[value-2],[value-3])
		$query = "INSERT INTO `user`(`userid`, `userpw`, `usernote`) VALUES ('$user','$pass','$memo')";
			//echo $query;

		$res = mysqli_query($conn, $query);	

		if($res)
		// break와 비슷. 긴급 탈출용
			die("Create Success. \n");
		else
			die("Create error. \n");
	}
	else
		die("Exists. \n");
	


	/*
	$rows = array();
	$result = array();

	while($row = mysqli_fetch_array($res))
	{
		$rows["ID"] = $row[0];//id
		$rows["PASS"] = $row[1];//password
		$rows["NOTE"]= $row[2];//note		
		array_push($result, $rows);
		//php에서 echo는 printf 와 같은 기능을한다.
		//출력 결과물을 화면에 표시해준다
		//echo $row[0];
	}
	echo json_encode(array("results"=>$result));
	*/
	mysqli_close($conn);	

?>