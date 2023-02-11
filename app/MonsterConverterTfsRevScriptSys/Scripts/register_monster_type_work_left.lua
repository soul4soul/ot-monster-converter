registerMonsterType.eventFile = function(mtype, mask) 
	if mask.eventFile then
		if type(mask.eventFile) == "boolean" then 
			mtype:setScriptFile()
		else
			mtype:setScriptFile(mask.eventFile)
		end
	end 
end
registerMonsterType.events = function(mtype, mask)
	if type(mask.events) == "table" then
		for k, v in pairs(mask.events) do
			mtype:registerEvent(v)
		end
	end
end
registerMonsterType.defenses = function(mtype, mask)
	if type(mask.defenses) == "table" then
		for _, defense in pairs(mask.defenses) do
			if type(defense) == "table" then
				local spell = MonsterSpell()
				if defense.name then
					spell:setType(defense.name)
					if defense.name == "melee" then
						if defense.attack and defense.skill then
							spell:setAttackValue(defense.attack, defense.skill)
						end
						if defense.condition then
							if defense.condition.type then
								spell:setConditionType(defense.condition.type)
							end
							local startDamnage = 0
							if defense.condition.startDamage then
								startDamage = defense.condition.startDamage
							end
							if defense.condition.minDamage and defense.condition.maxDamage then
								spell:setConditionDamage(defense.condition.minDamage, defense.condition.maxDamage, startDamage)
							end
							if defense.condition.duration then
								spell:setConditionDuration(defense.condition.duration)
							end
							if defense.condition.interval then
								spell:setConditionTickInterval(defense.condition.interval)
							end
						end
					else
						if defense.type then
							if defense.name == "combat" then
								spell:setCombatType(defense.type)
							else
								spell:setConditionType(defense.type)
							end
						end
						if defense.minDamage and defense.maxDamage then
							if defense.name == "combat" then
								spell:setCombatValue(defense.minDamage, defense.maxDamage)
							else
								local startDamage = 0
								if defense.startDamage then
									startDamage = defense.startDamage
								end
								spell:setConditionDamage(defense.minDamage, defense.maxDamage, startDamage)
							end
						end
					end
				end
				mtype:addDefense(spell)
			end
		end
	end
end
