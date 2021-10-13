<?php

	$user = $_POST["Input_user"];
	$pass = $_POST["Input_pass"];

	//error_reporting(E_ALL);
    //ini_set("display_errors", 1);

	//$user = '박성훈';
	$user = iconv('utf-8', 'utf-8', $user);
	//euc-kr > utf-8
	//$user = iconv('utf-8', 'utf-8', $user);
	//$memo 도 한국어가 들어올 수 있으므로, 인코딩 문제를 해결해줌
	
	$conn=mysqli_connect("localhost","root","1234");
		
	//if(mysqli_connect_errno($conn))
	//{
	//	echo "Fail to connect to MYSQL: " . msqli_connect_error();
	//}
	mysqli_set_charset($conn,"utf8");
	
	mysqli_select_db($conn, "study");

	//먼저 해당 아이디가 존재하는지 판단하기 위해서
	//SELECT 쿼리를 수행해줌
	$query = "SELECT * FROM `user` WHERE `userid`='$user'";

	//echo $query;

	$res = mysqli_query($conn, $query);	
	//결과값의 갯수를 numrows에 저장
	//결과값이 존재하지 않으면 0을 반환
	$numrows = mysqli_num_rows($res);    

	//echo $numrows;

	if($numrows > 0)
	{
		//DELETE FROM `user` WHERE 0
		$query = "DELETE FROM `user` WHERE `userid` = '$user'";
		echo $query;

		$res = mysqli_query($conn, $query);	

		if($res)
			die("Delete Success. \n");
		else
			die("Delete error. \n");
	}
	else
		die("Not Exists. \n");
	
	mysqli_close($conn);	
?>