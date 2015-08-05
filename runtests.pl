#!/usr/bin/perl

@wics   =  ("add.wic","mul.wic", "sub.wic", "div.wic", "and.wic", "and.wic",
            "or.wic", "or.wic",  "not.wic", "not.wic", "gcd.wic", "getsPushes.wic",
            "jump.wic","jumpf.wic", "jumpf.wic", "max.wic", "max.wic", 
            "testeq.wic", "testeq.wic", "testge.wic", "testge.wic",
            "testgt.wic", "testgt.wic", "testle.wic", "testle.wic",
            "testlt.wic", "testlt.wic", "testne.wic", "testne.wic"
);
@inputs =  ("2\n3\n", "2\n-3\n", "2\n-3\n", "4\n2\n",  "2\n3\n4", "4\n3\n2",
            "2\n3\n1", "1\n2\n3","3\n2",    "2\n3",    "",        "1\n2\n3\n4",
            "2\n3",    "2\n3",      "3\n2",      "2\n3",    "3\n2",
            "2\n3",       "3\n3",       "2\n3",       "3\n2",    
            "3\n3",       "4\n3",       "2\n3",       "3\n2",    
            "3\n3",       "2\n3",       "2\n3",       "3\n3"    
);
@counts =  ("1",      "1",       "1",       "1",       "1",       "2",
            "1",      "2",       "1",       "2",       "1",       "1",
            "1",      "1",       "2",       "1",       "2",
            "1",      "2",       "1",       "2",
            "1",      "2",       "1",       "2",
            "1",      "2",       "1",       "2"
);

if (! -e "TestResults/")
{
   print "need to create a TestResults directory first\n";
   exit();
}

$pass = 0;
$candir = "./Inputs/";
$canwic = "./wici_reference.exe";
$studentwic = "./WICI.exe";

system "rm -f TestResults/*";

for ($i = 0; $i <= $#wics; $i++)
{
   $wic = $candir.$wics[$i];      
   $echostr = "\"". $inputs[$i]. "\"";       
   $inputfile = $wics[$i].$counts[$i].".in";  
   print "Testing $wics[$i].";
   system "echo $echostr > $inputfile";
   $canoutputfile = $wics[$i] . $counts[$i] . ".can.out";   
   $studentoutputfile = $wics[$i] . $counts[$i] . ".student.out";
   system "$canwic $wic < $inputfile > $canoutputfile";
   system "$studentwic $wic < $inputfile > $studentoutputfile";
   
   if (-e $studentoutputfile)
   {
      system "diff -b $canoutputfile $studentoutputfile > difffile";
      $problemFileSize = -s "difffile";
   }
   
   if (!(-e $studentoutputfile)) 
   {
      # didn't create an output file
      print " Failed.\n";    
      system "cp $wic TestResults/";
      system "mv $inputfile TestResults/";
      system "mv $canoutputfile TestResults/";
  } elsif ($problemFileSize > 0)
  {
      print " Failed.\n";    
      system "cp $wic TestResults/";
      system "mv $inputfile TestResults/";
      system "mv $canoutputfile TestResults/";
      system "mv $studentoutputfile TestResults/";
  } else
  {
      print " Passed.\n";    
      system "rm -rf $canoutputfile $studentoutputfile $inputfile";
      $pass = $pass + 1;
  }
  system "rm -rf difffile";
}

$total = $#wics + 1;
print "\n$pass out of $total passed.\n";

if ($pass != $total) 
{
   print "See TestResults directory for failed TestResults\n";
}


