package main

import (
	"fmt"
	"os"
	"os/exec"
	"strconv"
	"syscall"
	"path/filepath"
	"path"
)

var version string = "1.0.6545"
var dotnet string

func printWelcome() {
	fmt.Println(fmt.Sprintf("Xeora Web Development Framework CLI v%s", version))
	fmt.Println()
}

func main() {
	outputBytes, err := exec.Command("dotnet", "--version").Output()
	if err != nil {
		printWelcome()

		dotnetURL := fmt.Sprintf("https://www.microsoft.com/net/learn/get-started/%s", dotnet)
		fmt.Println("Please install .NET Core 2.0 or higher version!")
		fmt.Println(dotnetURL)

		os.Exit(1)
	}
	outputString := string(outputBytes)

	dotnetVersion, err := strconv.ParseInt(outputString[:1], 10, 64)
	if dotnetVersion < 2 {
		printWelcome()
		fmt.Println("Please upgrade .NET Core to version 2.0 or higher!")

		os.Exit(2)
	}

	xeoraCLIAssm := getXeoraPath()
	if xeoraCLIAssm != "" {
		xeoraCLIAssm = path.Join(xeoraCLIAssm, "Xeora.CLI.dll")
		if _, err := os.Stat(xeoraCLIAssm); os.IsNotExist(err) {
			xeoraCLIAssm = ""
		}
	}

	if xeoraCLIAssm == "" {
		printWelcome()
		fmt.Println("Please setup XEORAPATH environment variable properly!")

		os.Exit(3)
	}

	result := handleArgs(xeoraCLIAssm, os.Args[1:])
	os.Exit(result)
}

func getXeoraPath() string {
	xeoraPath := os.Getenv("XEORAPATH")
	if xeoraPath == "" {
		xeoraPath, _ = filepath.Abs(filepath.Dir(os.Args[0]))
	}

	return xeoraPath
}

func handleArgs(xeoraPath string, args []string) int {
	if len(args) == 0 {
		args = append(args, "dummy")
	}

	switch args[0] {
	case "-v", "--version":
		fmt.Println(version)

		return 0
	default:
		return execute(xeoraPath, args)
	}
}

func execute(xeoraCLIAssembly string, args []string) int {
	var result int = 0

	var cliArguments []string
	cliArguments = append(cliArguments, xeoraCLIAssembly)
	cliArguments = append(cliArguments, args...)

	cmd := exec.Command("dotnet", cliArguments...)
	stdout, _ := cmd.StdoutPipe()
	err := cmd.Start()
	if err != nil {
		result = 1

		if exitError, ok := err.(*exec.ExitError); ok {
			ws := exitError.Sys().(syscall.WaitStatus)

			result = ws.ExitStatus()
		}
	}

	lastWrittenCount := 0
	bR := 0
	buffer := make([]byte, 1024)
	for {
		bR, err = stdout.Read(buffer)
		if err != nil {
			break
		}

		totalPrinted := printPossible(buffer, bR)
		if lastWrittenCount > 0 {
			lastWrittenCount += totalPrinted
		}
		lastWrittenCount = printRepeating(lastWrittenCount, buffer, bR)
	}

	return result
}

func printPossible(buffer []byte, count int) int {
	lastIndex := -1

	for rC := 0; rC < count; rC++ {
		if buffer[rC] == '\t' {
			if lastIndex > -1 {
				fmt.Print(string(buffer[lastIndex:rC]))
				return rC
			}
			return 0
		}

		if buffer[rC] == '\n' {
			if lastIndex == -1 {
				lastIndex = 0
			}
			fmt.Println(string(buffer[lastIndex:rC]))
			lastIndex = rC + 1
		}
	}
	if lastIndex == -1 {
		lastIndex = 0
	}
	fmt.Print(string(buffer[lastIndex:count]))
	return count - lastIndex
}

func printRepeating(lastWrittenCount int, buffer []byte, count int) int {
	tabIndex := -1

	for rC := count - 1; rC > -1; rC-- {
		if buffer[rC] == '\t' {
			tabIndex = rC;
			break;
		}
	}

	if tabIndex > -1 {
		fmt.Printf("\033[%dD", lastWrittenCount)
		fmt.Print(string(buffer[tabIndex:count]))
		lastWrittenCount = count - tabIndex
	}

	return lastWrittenCount
}
