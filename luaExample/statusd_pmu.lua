-- A battery statusbar meter for the ion3 window manager for Apple Powerbooks
-- running Linux. These machines do not use APM or ACPI but have a PMU, Power
-- Management Unit.
--
-- Install the meter in ~/.ion3/statusd_pmu.lua
--
-- Edit your ~/.ion3/cfg_statusbar.lua to add this meter: 
--   template="%date - %pmu_battery %filler%systray",
--
-- When the battery is charged and you're on AC power, you don't see anything.
-- When the system is on AC power and charging the battery, you'll see a tilde
-- sign, ~, followed by the percentage that the battery is charged.  When the
-- system is running on battery power, you'll an equal, =, sign, followed by
-- the remaining capacity in percentages. 
--
-- See /usr/src/linux/include/pmu.h and
-- /usr/src/linux/drivers/macintosh/via-pmu.c for some documentation on the
-- exported data. 
--
-- Author: Jeroen Pulles
-- Rotterdam, 15 november 2007


local pmu_base_settings = {
  update_interval = 60*1000, -- every minute
  important_threshold = 33 , -- 33% cap. remaining
  critical_threshold = 8, -- 8% capacity remaining
}

local pmu_settings = pmu_base_settings

local pmu_timer


-- Read the pmu battery info
local function read_pmu_battery_data ()
  -- assume only one of possible two batteries is present:
  local f = assert(io.open("/proc/pmu/battery_0", "r"))
  local data = f:read("*all")
  f:close()
  local _, _, charge = string.find(data, "[^_]charge%s*:%s*(%d+)")
  local _, _, max_charge = string.find(data, "max_charge%s*:%s*(%d+)")
  local _, _, amperage = string.find(data, "current%s*:%s*(-?[%d]+)")
  return tonumber(amperage), tonumber(charge), tonumber(max_charge)
end


-- Convert the battery data to usable info:
local function get_pmu_battery_state ()
  local amp, charge, max_charge = read_pmu_battery_data()
  local pct = (charge / max_charge) * 100
  if amp == 0 then 
    -- charge doesn't always go to exactly full max charge
    -- but the laptop stops charging anyway: 
    pct = 100
  end
  return amp, pct
end


-- Print the battery status to stdout, for debugging purposes:
local function print_batt_info ()
  local amp, pct = get_pmu_battery_state()
  local state = ""
  if amp > 0 then
    state = "Charging "
  elseif amp < 0 then
    state = "Remaining "
  end
  print(string.format("%s%d%%", state, pct))
end


-- Write the current state to the statusbar:
local function inform_pmu ()

  local amp, pct = get_pmu_battery_state()

  if amp == 0 then
    -- Do no show anything when on AC power and fully charged: 
    statusd.inform("pmu_battery", "")
  elseif amp > 0 then
    -- Charging the battery on AC power, percentage charged:
    statusd.inform("pmu_battery", string.format("~%d%%", pct))
  else
    -- On battery power, percentage remaining: 
    statusd.inform("pmu_battery", string.format("=%d%%", pct))
  end

  -- crit and imp. hints only when on battery: 
  if amp < 0 and pct <= pmu_settings.critical_threshold then
    statusd.inform("pmu_battery_hint", "critical")
  elseif amp < 0 and pct <= pmu_settings.important_threshold then
    statusd.inform("pmu_battery_hint", "important")
  else
    statusd.inform("pmu_battery_hint", "normal")
  end

end


-- Statusbar update loop:
local function update_pmu ()
  inform_pmu()
  pmu_timer:set(pmu_settings.update_interval, update_pmu)
end


-- Run the script: 
if statusd then 
  -- we're a statusbar plugin:
  pmu_settings = table.join(statusd.get_config("pmu"), pmu_base_settings)
  pmu_timer = statusd.create_timer()
  update_pmu()
else
  -- run as standalone:
  print_batt_info()

end

-- vim: set ts=4 sw=4 expandtab
