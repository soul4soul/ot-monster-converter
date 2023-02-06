function createFunctions(class)
	local exclude = {"get", "set", "is", "add", "can"}
	local temp = {}
	for name, func in pairs(class) do
		if not table.contains(exclude, name:sub(1,3)) then
			local str = name:sub(1,1):upper()..name:sub(2)
			local getFunc = function(self) return func(self) end
			local setFunc = function(self, ...) return func(self, ...) end
			local get = "get".. str
			local set = "set".. str
			table.insert(temp, {set, setFunc, get, getFunc})
		end
	end
	for _,func in ipairs(temp) do
		rawset(class, func[1], func[2])
		rawset(class, func[3], func[4])
	end
end