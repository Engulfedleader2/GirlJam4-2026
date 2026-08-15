// Flavor text pools for death receipts - mix a Description with a While to
// get something like "Dave was strangled by their own scarf while fleeing a skeleton."
//
// "While" only holds enemy-agnostic activities. When we know which enemy
// actually killed the adventurer, DungeonManager builds an accurate
// "fleeing {EnemyName}" line instead, so the message never names a monster
// that isn't the one they actually died to.

public static class DeathMessages
{
	public static readonly string[] Descriptions =
	{
		"strangled by their own scarf",
		"tripped on a rock",
		"overwhelmed with fear",
		"allergic reaction",
		"breathe too much dust",
		"bleed to death from a paper cut",
		"drowned",
		"pricked to death",
		"dehydrated",
		"blew up",
		"hit the ground too hard",
		"choke on a vine",
		"impaled",
		"went up in flame",
		"shot up",
		"chop up",
		"struck by lighting",
		"freeze to death",
		"discovered the floor was lava",
		"killed by magic",
		"starved to death",
		"found out their pants was just too tight",
		"found out they should have layerd their clothes more",
		"forgot to cherish their friendship",
		"forgot to brush their teeth"
	};

	public static readonly string[] While =
	{
		"cooking",
		"resting",
		"relieving themselves",
		"sleeping",
		"walking",
		"running",
		"reading",
		"dreaming",
		"sitting",
		"standing",
		"talking",
		"reading",
		"jogging",
		"hiding",
		"crouching",
		"negotiating",
		"stretching",
		"fleeing",
		"fleeing a trap",
		"fleeing from their responsibility"
	};
}
