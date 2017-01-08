# GenericSearch.ps1
# PowerShell program to search Active Directory.
# Author: Richard Mueller
# PowerShell Version 1.0
# August 16, 2011
# March 20, 2013 - Modify rounding of local time zone bias,
#                  Fix Function OctetToGUID.
# May 1, 2013 - Account for negative time zone bias.
# May 2, 2013 - Handle multi-line strings in csv format.

Trap {"Error: $_"; Break;}

$Colon = ":"
# Check optional parameter indicating output should be in csv format.
$Csv = $False
If ($Args.Count -eq 1)
{
    If (($Args[0].ToLower() -eq "/csv") -or ($Args[0].ToLower() -eq "csv"))
    {$Csv = $True}
}

# Retrieve local Time Zone bias from machine registry in hours.
# This bias does not change with Daylight Savings Time.
$Bias = (Get-ItemProperty `
    -Path HKLM:\System\CurrentControlSet\Control\TimeZoneInformation).Bias
# Account for negative bias.
If ($Bias -gt 10080){$Bias = $Bias - 4294967296}
$Bias = [Math]::Round($Bias/60, 0, [MidpointRounding]::AwayFromZero)

# Create an array of 168 bytes, representing the hours in a week.
$LH = New-Object 'object[]' 168

Function OctetToGUID ($Octet)
{
    # Function to convert Octet value (byte array) into string GUID value.
    $GUID = [GUID]$Octet
    Return $GUID.ToString("B")
}

Function OctetToHours ($Octet)
{
    # Function to convert Octet value (byte array) into binary string
    # representing logonHours attribute. The 168 bits represent 24 hours
    # per day for 7 days, Sunday through Saturday. The values are converted
    # into local time. If the bit is "1", the user is allowed to logon
    # during that hour. If the bit is "0", the user is not allowed to logon.
    For ($j = 0; $j -le 20; $j = $j + 1)
    {
        For ($k = 7; $k -ge 0; $k = $k - 1)
        {
            $m = 8*$j + $k - $Bias
            If ($m -lt 0) {$m = $m + 168}
            If ($m -gt 167) {$m = $m - 168}
            If ($Octet[$j] -band [Math]::Pow(2, $k)) {$LH[$m] = "1"}
            Else {$LH[$m] = "0"}
        }
    }

    For ($j = 0; $J -le 20; $j = $J + 1)
    {
        $n = 8*$j
        If ($j -eq 0) {$Hours = [String]::Join("", $LH[$n..($n + 7)])}
        Else {$Hours = $Hours + "-" + [String]::Join("", $LH[$n..($n + 7)])}
    }
    Return $Hours
}

$Searcher = New-Object System.DirectoryServices.DirectorySearcher
$Searcher.PageSize = 200
$Searcher.SearchScope = "subtree"

# Prompt for base of query.
$BaseDN = Read-Host "Enter DN of base of query, or blank for entire domain"
If ($BaseDN -eq "")
{
    # Default to the entire domain.
    $Base = New-Object System.DirectoryServices.DirectoryEntry
}
Else
{
    If ($BaseDN.ToLower().Contains("dc=") -eq $False)
    {
        $Domain = New-Object System.DirectoryServices.DirectoryEntry
        $BaseDN = $BaseDN + "," + $Domain.distinguishedName
        $BaseDN = $BaseDN.Replace(",,", ",")
    }
    $Base = New-Object System.DirectoryServices.DirectoryEntry "LDAP://$BaseDN"
}
$Searcher.SearchRoot = $Base

# Prompt for LDAP syntax filter.
$Filter = Read-Host "Enter LDAP syntax filter"
If ($Filter.StartsWith("(") -eq $False) {$Filter = "(" + $Filter}
If ($Filter.EndsWith(")") -eq $False) {$Filter = $Filter + ")"}
$Searcher.Filter = $Filter

# Prompt for attributes.
$Attributes = Read-Host "Enter comma delimited list of attribute values to retrieve"
# Remove any spaces.
$Attributes = $Attributes -replace " ", ""
$arrAttrs = $Attributes.Split(",")
$Searcher.PropertiesToLoad.Add("distinguishedName") > $Null
ForEach ($Attr In $arrAttrs)
{
    If ($Attr -ne "") { $Searcher.PropertiesToLoad.Add($Attr) > $Null }
}

If ($Csv -eq $False)
{
    "Base of query: " + $Base.distinguishedName
    "Filter: $Filter"
    "Attributes: $Attributes"
    "----------------------------------------------"
}
Else
{
    # Header line.
    $Line = "DN"
    ForEach ($Attr In $arrAttrs)
    {
        If ($Attr -ne "") { $Line = $Line + "," + $Attr }
    }
    $Line
}

# Run the query.
$Results = $Searcher.FindAll()

# Enumerate resulting recordset.
$Count = 0
ForEach ($Result In $Results)
{
    $Count = $Count + 1
    $DN = $Result.Properties.Item("distinguishedName")
    If ($Csv -eq $True)
    {
        # Any double quote characters in the DN must be doubled.
        $Line = """" + $DN[0].Replace("""", """""") + """"
    }
    Else
    {
        "DN: " + $DN
    }
    # Retrieve all requested attributes.
    ForEach ($Attr In $arrAttrs)
    {
        If ($Attr -ne "")
        {
            $Values = $Result.Properties.Item($Attr)
            If ($Values[0] -eq $Null)
            {
                # Attribute has no value.
                If ($Csv -eq $True) {$Line = "$Line,<no value>"}
                Else {"  $Attr$Colon <no value>"}
            }
            Else
            {
                # Attribute might be multi-valued. Values will be semicolon delimited.
                # Values will only be quoted if they are String.
                $Multi = ""
                $Quote = $False
                ForEach ($Value In $Values)
                {
                    Switch ($Value.GetType().Name)
                    {
                        "Int64"
                        {
                            # Attribute is Integer8 (64-bit).
                            If ($Value -gt 9000000000000000000)
                            {
                                # Value is maximum 64-bit value, 2^63 - 1.
                                If ($Csv -eq $True)
                                    {
                                    If ($Multi -eq "") {$Multi = "<never>"}
                                    Else {$Multi = "$Multi;<Never>"}
                                }
                                Else {"  $Attr$Colon <never>"}
                            }
                            Else
                            {
                                If ($Value -gt 120000000000000000)
                                {
                                    # Integer8 value is a date, greater than
                                    # April 07, 1981, 9:20 PM UTC.
                                    $Date = [Datetime]$Value
                                    If ($Csv -eq $True)
                                    {
                                        If ($Multi -eq "")
                                        {
                                            $Multi = $Date.AddYears(1600).ToLocalTime()
                                        }
                                        Else
                                        {
                                            $Multi = "$Multi;" `
                                                + $Date.AddYears(1600).ToLocalTime()
                                        }
                                    }
                                    Else
                                    {
                                        "  $Attr$Colon " + '{0:n0}' -f $Value `
                                            + " (" + $Date.AddYears(1600).ToLocalTime() + ")"
                                    }
                                }
                                Else
                                {
                                    # Integer8 value, not a date.
                                    If ($Csv -eq $True)
                                    {
                                        If ($Multi -eq "") {$Multi = '{0:n0}' -f $Value}
                                        Else {$Multi = "$Multi;" + '{0:n0}' -f $Value}
                                    }
                                    Else {"  $Attr$Colon " + '{0:n0}' -f $Value}
                                }
                            }
                        }
                        "Byte[]"
                        {
                            # Attribute is a byte array (OctetString).
                            If (($Value.Length -eq 16) `
                                -and ($Attr.ToUpper().Contains("GUID") -eq $True))
                            {
                                # GUID value.
                                If ($Csv -eq $True)
                                {
                                    If ($Multi -eq "") {$Multi = $(OctetToGUID $Value)}
                                    Else {$Multi = "$Multi;" + $(OctetToGUID $Value)}
                                }
                                Else {"  $Attr$Colon " + $(OctetToGUID $Value)}
                            }
                            Else
                            {
                                If (($Value.Length -eq 21) -and ($Attr -eq "logonHours"))
                                {
                                    # logonHours attribute, byte array of 168 bits.
                                    # One binary bit for each hour of the week, in UTC.
                                    If ($Csv -eq $True)
                                    {
                                        If ($Multi -eq "") {$Multi = $(OctetToHours $Value)}
                                        Else {$Multi = "$Multi;" + $(OctetToHours $Value)}
                                    }
                                    Else {"  $Attr$Colon " + $(OctetToHours $Value)}
                                }
                                Else
                                {
                                    If (($Value[0] -eq 1) -and (`
                                        (($Value[1] -eq 1) -and ($Value.Length -eq 12)) `
                                        -or (($Value[1] -eq 2) -and ($Value.Length -eq 16)) `
                                        -or (($Value[1] -eq 4) -and ($Value.Length -eq 24)) `
                                        -or (($Value[1] -eq 5) -and ($Value.Length -eq 28))))
                                    {
                                        # SID value.
                                        $SID = New-Object System.Security.Principal.SecurityIdentifier $Value, 0
                                        If ($Csv -eq $True)
                                        {
                                            If ($Multi -eq "") {$Multi = $SID}
                                            Else {$Multi = "$Multi;$SID"}
                                        }
                                        Else {"  $Attr$Colon $SID"}
                                    }
                                    Else
                                    {
                                        # Byte array.
                                        If ($Csv -eq $True)
                                        {
                                            If ($Multi -eq "") {$Multi = $Value}
                                            Else {$Multi = "$Multi;$Value"}
                                        }
                                        Else {"  $Attr$Colon $Value"}
                                    }
                                }
                            }
                        }
                        "String"
                        {
                            # String value. Enclose in quotes in case there are embedded
                            # commas. Any double quote characters in the string must
                            # be doubled.
                            $Quote = $True
                            If ($Csv -eq $True)
                            {
                                # Embedded quotes must be doubled.
                                $Value = $Value.Replace("""", """""")
                                # Multi-line values must have carriage return line
                                # feed characters replaced with ";".                         
                                $Value = $Value.Replace("`r`n", ";")
                                If ($Multi -eq "") {$Multi = $Value}
                                Else {$Multi = "$Multi;$Value"}
                            }
                            Else {"  $Attr$Colon $Value"}
                        }
                        "Int32"
                        {
                            # 32-bit integer.
                            If ($Csv -eq $True)
                            {
                                If ($Multi -eq "") {$Multi = "$Value"}
                                Else {$Multi = "$Multi;$Value"}
                            }
                            Else {"  $Attr$Colon $Value"}
                        }
                        "Boolean"
                        {
                            # Boolean value.
                            If ($Csv -eq $True)
                            {
                                If ($Multi -eq "") {$Multi = "$Value"}
                                Else {$Multi = "$Multi;$Value"}
                            }
                            Else {"  $Attr$Colon $Value"}
                        }
                        "DateTime"
                        {
                            # Datetime value.
                            If ($Csv -eq $True)
                            {
                                If ($Multi -eq "") {$Multi = "$Value"}
                                Else {$Multi = "$Multi;$Value"}
                            }
                            Else {"  $Attr$Colon $Value"}
                        }
                        Default
                        {
                            If ($Csv -eq $True)
                            {
                                If ($Multi -eq "") {$Multi = "<not supported> (" + $Value.GetType().Name + ")"}
                                Else {Multi = "$Multi;<not supported> (" + $Value.GetType().Name + ")"}
                            }
                            Else {"  $Attr$Colon <not supported> (" + $Value.GetType().Name + ")"}
                        }
                    }
                }
                If ($Csv -eq $True)
                {
                    # Enclose values in double quotes if necessary.
                    If ($Quote -eq $True) {$Line = "$Line,""$Multi"""}
                    Else {$Line = "$Line,$Multi"}
                }
            }
        }
    }
    If ($Csv -eq $True) {$Line}
}

If ($Csv -eq $False) {"Number of objects found: $Count"}