# SA Chat Mute Fix v2.0

🔒 **Enhanced Counter-Strike 2 plugin that completely eliminates chat bypass in CS2-SimpleAdmin GAG/MUTE punishments**

[![GitHub release (latest by date)](https://img.shields.io/github/v/release/Franqowsky/SA-Chat-Mute-Fix-v2)](https://github.com/Franqowsky/SA-Chat-Mute-Fix-v2/releases)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-Compatible-green)](https://github.com/roflmuffin/CounterStrikeSharp)

## 🚨 The Problem

CS2-SimpleAdmin has a critical vulnerability where muted/gagged players can bypass chat restrictions:

```bash
# Admin mutes player
css_gag BadPlayer 60 "Toxic behavior"

# Player can still communicate using prefixes
!admin "This is unfair!"
/help "I need assistance"
say_team "Can anyone hear me?"
```

**Result**: Other players see these messages, making punishments completely ineffective.

## ✅ The Solution

SA Chat Mute Fix v2.0 provides **bulletproof protection** by:

- 🛡️ **Complete chat blocking** - No message gets through, regardless of prefix
- 🔄 **Real-time database sync** - Automatic connection to your SimpleAdmin database
- ⚡ **Zero configuration** - Reads your existing SimpleAdmin settings automatically
- 🎯 **Multi-punishment support** - Blocks GAG, MUTE, and SILENCE violations
- 📊 **Detailed logging** - Full transparency of blocked attempts

## 🎥 Before vs After

| Before SA Chat Mute Fix | After SA Chat Mute Fix |
|------------------------|------------------------|
| ❌ `!admin` messages visible to all | ✅ All messages completely blocked |
| ❌ `/help` commands bypass mute | ✅ Commands intercepted and stopped |
| ❌ Team chat still works | ✅ Team chat also blocked |
| ❌ Punishment ineffective | ✅ 100% effective punishment |

## 📋 Features

### 🔧 Core Functionality
- **Complete message interception** - Hooks all chat events and user messages
- **Database auto-detection** - Automatically finds and uses SimpleAdmin's MySQL configuration
- **Real-time monitoring** - Updates penalty cache every 5 seconds
- **Multi-format support** - Handles both DATETIME and Unix timestamp formats
- **Permanent penalty support** - Correctly handles NULL end dates

### 🛠️ Technical Excellence
- **Thread-safe operations** - Async database queries with proper error handling
- **Memory efficient** - Smart caching system with automatic cleanup
- **Performance optimized** - <1ms processing time per message
- **Extensive logging** - Detailed information for troubleshooting
- **Backward compatible** - Works with SimpleAdmin 1.7.7+ versions

## 🔧 Requirements

| Component | Version | Status |
|-----------|---------|--------|
| **CounterStrikeSharp** | Latest | ✅ Required |
| **CS2-SimpleAdmin** | 1.7.7+ | ✅ Required |
| **MySQL/MariaDB** | Any | ✅ Required |
| **.NET Runtime** | 8.0+ | ✅ Auto-included |

## 📦 Installation

### 1. Download & Extract
```bash
# Download from releases
wget https://github.com/Franqowsky/SA-Chat-Mute-Fix-v2/releases/latest/download/SAChatMuteFix.zip

# Extract to plugins directory
unzip SAChatMuteFix.zip -d /path/to/csgo/addons/counterstrikesharp/plugins/
```

### 2. File Structure
```
csgo/addons/counterstrikesharp/plugins/SAChatMuteFix/
├── SAChatMuteFix.dll
├── MySqlConnector.dll          # ⚠️ Important: Required dependency
├── [additional dependencies]
```

### 3. Verification
```bash
# Restart your CS2 server, then check:
css_plugins list

# Expected output:
# SA Chat Mute Fix (2.0.5) by Franqowsky [Loaded]
```

## ⚙️ Configuration

**🎉 ZERO CONFIGURATION REQUIRED!**

The plugin automatically:
1. 🔍 Locates your SimpleAdmin configuration file
2. 📖 Reads database connection settings from `CS2-SimpleAdmin.json`
3. 🔌 Connects to your existing MySQL database
4. ▶️ Starts monitoring the `sa_mutes` table

### Configuration File Location
```
csgo/addons/counterstrikesharp/configs/plugins/CS2-SimpleAdmin/CS2-SimpleAdmin.json
```

### Supported Configuration Structure
```json
{
  "DatabaseConfig": {
    "DatabaseType": "mysql",
    "DatabaseHost": "127.0.0.1",
    "DatabasePort": 3306,
    "DatabaseUser": "cs2user",
    "DatabasePassword": "your_password",
    "DatabaseName": "simpleadmin",
    "DatabaseSSlMode": "preferred"
  }
}
```

## 🎯 Usage Examples

### Admin Actions
```bash
# Standard SimpleAdmin commands work as usual
css_gag PlayerName 30 "Spamming"
css_mute ToxicPlayer 60 "Verbal abuse" 
css_silence Cheater 120 "Disruptive behavior"
```

### Plugin Response
```log
[INFO] 🚀 SA Chat Mute Fix v2.0.5 loaded with conn: Server=127.0.0.1;Database=simpleadmin;...
[INFO] 🚫 BLOCKED chat from PlayerName
[INFO] 🚫 BLOCKED chat from ToxicPlayer
```

### For Players
- ✅ **Muted players see**: Your message appears to send normally
- ✅ **Other players see**: Nothing (message completely blocked)
- ✅ **Admins see**: Detailed logs of blocked attempts

## 🐛 Troubleshooting

### Common Issues

<details>
<summary><b>🔴 Plugin not loading</b></summary>

**Check these items:**
- CounterStrikeSharp is properly installed
- All DLL dependencies are present in plugin folder
- Server has been restarted after installation
- Check server console for error messages

```bash
# Verify CSS is working
css_plugins list

# Check plugin specific errors
grep "SA Chat Mute Fix" path/to/server/logs/
```
</details>

<details>
<summary><b>🔴 Database connection failed</b></summary>

**Verify database access:**
- SimpleAdmin is working correctly
- Database credentials are correct
- MySQL server is accessible
- `sa_mutes` table exists

```bash
# Test SimpleAdmin database connection
css_addban test 1 "Connection test"
css_unban test
```
</details>

## 🔧 Development

### Building from Source
```bash
# Clone repository
git clone https://github.com/Franqowsky/SA-Chat-Mute-Fix-v2.git
cd SA-Chat-Mute-Fix-v2

# Build release version
dotnet build -c Release

# Output location
ls build/SAChatMuteFix.dll
```

## 🤝 Contributing

We welcome contributions! Here's how to help:

1. **🍴 Fork** this repository
2. **🌿 Create** your feature branch (`git checkout -b feature/amazing-feature`)
3. **💻 Code** your improvements
4. **✅ Test** thoroughly on a CS2 server
5. **📝 Commit** your changes (`git commit -m 'Add amazing feature'`)
6. **🚀 Push** to the branch (`git push origin feature/amazing-feature`)
7. **🔄 Open** a Pull Request

## 🏆 Version History

### v2.0.5 (Latest)
- ✅ Complete rewrite with enhanced database integration
- ✅ Auto-detection of SimpleAdmin configuration
- ✅ Improved error handling and logging
- ✅ Support for all SimpleAdmin database formats
- ✅ Zero-configuration setup

### v1.0.4 (Legacy)
- ✅ Basic chat blocking functionality
- ✅ Manual configuration required
- ⚠️ Limited database format support

## 💡 Credits & Acknowledgments

- **👨‍💻 Author**: [Franqowsky](https://github.com/Franqowsky)
- **🎯 Inspired by**: [CS2-SimpleAdmin](https://github.com/daffyyyy/CS2-SimpleAdmin) team's excellent admin management system
- **🛠️ Built with**: [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) framework
- **🗄️ Database**: MySQL integration for real-time penalty tracking

## 🆘 Support & Community

- **🐛 Bug Reports**: [GitHub Issues](https://github.com/Franqowsky/SA-Chat-Mute-Fix-v2/issues)
- **💬 Discussions**: [GitHub Discussions](https://github.com/Franqowsky/SA-Chat-Mute-Fix-v2/discussions)
- **🎮 Server**: connect 172.235.90.136:27015

## 📄 License

This project is licensed under the **MIT License** - see the [LICENSE](LICENSE) file for details.

---

### 🌟 **Star this repository if SA Chat Mute Fix helped secure your server!**

**Made with ❤️ for the Counter-Strike 2 community**
