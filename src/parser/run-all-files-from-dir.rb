if ARGV.count == 0
	puts 'Must specify directory'
	exit
end

dirname = ARGV[0]
if !dirname.end_with? File::SEPARATOR or !dirname.end_with? File::ALT_SEPARATOR 
	dirname += File::SEPARATOR
end

	# replace \ for /
dirname.gsub!(File::ALT_SEPARATOR, File::SEPARATOR)

globexp = dirname+"**/*.pas"

dirs = Dir.glob(globexp).reject{|x| File.directory?(x)}.join(' ')

#puts outp = %x[ parser-delphi.exe #{dirs} ]

system 'parser-delphi.exe '+dirs
