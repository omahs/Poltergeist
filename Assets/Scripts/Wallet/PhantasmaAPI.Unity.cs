using System;
using System.Collections;
using System.Globalization;

using UnityEngine;

using LunarLabs.Parser;

using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Phantasma.Core.Cryptography;
using Phantasma.Core.Numerics;
using Phantasma.Core.Domain;
using Phantasma.Business.Blockchain.Storage;
using Phantasma.Shared.Types;
using System.Threading.Tasks;

namespace Phantasma.SDK
{
    internal static class PhantasmaAPIUtils
    {
        internal static long GetInt64(this DataNode node, string name)
        {
            return node.GetLong(name);
        }

        internal static bool GetBoolean(this DataNode node, string name)
        {
            return node.GetBool(name);
        }
    }

    public struct Balance
    {
        public string chain; //
        public string amount; //
        public string symbol; //
        public uint decimals; //
        public string[] ids; //

        public static Balance FromNode(DataNode node)
        {
            Balance result;

            result.chain = node.GetString("chain");
            result.amount = node.GetString("amount");
            result.symbol = node.GetString("symbol");
            result.decimals = node.GetUInt32("decimals");
            var ids_array = node.GetNode("ids");
            if (ids_array != null)
            {
                result.ids = new string[ids_array.ChildCount];
                for (int i = 0; i < ids_array.ChildCount; i++)
                {

                    result.ids[i] = ids_array.GetNodeByIndex(i).AsString();
                }
            }
            else
            {
                result.ids = new string[0];
            }


            return result;
        }
    }

    public struct Interop
    {
        public string local; //
        public string external; //

        public static Interop FromNode(DataNode node)
        {
            Interop result;

            result.local = node.GetString("local");
            result.external = node.GetString("external");

            return result;
        }
    }

    public struct Platform
    {
        public string platform; //
        public string chain; //
        public string fuel; //
        public string[] tokens; //
        public Interop[] interop; //

        public static Platform FromNode(DataNode node)
        {
            Platform result;

            result.platform = node.GetString("platform");
            result.chain = node.GetString("chain");
            result.fuel = node.GetString("fuel");
            var tokens_array = node.GetNode("tokens");
            if (tokens_array != null)
            {
                result.tokens = new string[tokens_array.ChildCount];
                for (int i = 0; i < tokens_array.ChildCount; i++)
                {

                    result.tokens[i] = tokens_array.GetNodeByIndex(i).AsString();
                }
            }
            else
            {
                result.tokens = new string[0];
            }

            var interop_array = node.GetNode("interop");
            if (interop_array != null)
            {
                result.interop = new Interop[interop_array.ChildCount];
                for (int i = 0; i < interop_array.ChildCount; i++)
                {

                    result.interop[i] = Interop.FromNode(interop_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.interop = new Interop[0];
            }


            return result;
        }
    }

    public struct Swap
    {
        public string sourcePlatform; //
        public string sourceChain; //
        public string sourceHash; //
        public string sourceAddress; //
        public string destinationPlatform; //
        public string destinationChain; //
        public string destinationHash; //
        public string destinationAddress; //
        public string symbol; //
        public string value; //

        public static Swap FromNode(DataNode node)
        {
            Swap result;

            result.sourcePlatform = node.GetString("sourcePlatform");
            result.sourceChain = node.GetString("sourceChain");
            result.sourceHash = node.GetString("sourceHash");
            result.sourceAddress = node.GetString("sourceAddress");
            result.destinationPlatform = node.GetString("destinationPlatform");
            result.destinationChain = node.GetString("destinationChain");
            result.destinationHash = node.GetString("destinationHash");
            result.destinationAddress = node.GetString("destinationAddress");
            result.symbol = node.GetString("symbol");
            result.value = node.GetString("value");

            return result;
        }
    }

    public struct Stake
    {
        public string amount; //
        public uint time; //
        public string unclaimed; //

        public static Stake FromNode(DataNode node)
        {
            Stake result;

            result.amount = node.GetString("amount");
            result.time = node.GetUInt32("time");
            result.unclaimed = node.GetString("unclaimed");

            return result;
        }
    }

    public struct Storage
    {
        public uint available;
        public uint used; //
        public string avatar; //
        public Archive[] archives; //

        public static Storage FromNode(DataNode node)
        {
            Storage result;

            if (node == null)
            {
                result.archives = null;
                result.available = 0;
                result.avatar = null;
                result.used = 0;
                return result;
            }

            result.available = node.GetUInt32("available");
            result.used = node.GetUInt32("used");
            result.avatar = node.GetString("avatar");

            var archive_array = node.GetNode("archives");
            if (archive_array != null)
            {
                result.archives = new Archive[archive_array.ChildCount];
                for (int i = 0; i < archive_array.ChildCount; i++)
                {

                    result.archives[i] = Archive.FromNode(archive_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.archives = new Archive[0];
            }

            return result;
        }
    }

    public struct Account
    {
        public string address; //
        public string name; //
        public Stake stake; //
        public Storage storage; //
        public string relay; //
        public string validator; //
        public Balance[] balances; //

        public static Account FromNode(DataNode node)
        {
            Account result;

            result.address = node.GetString("address");
            result.name = node.GetString("name");
            result.stake = Stake.FromNode(node.GetNode("stakes"));
            result.storage = Storage.FromNode(node.GetNode("storage"));
            result.relay = node.GetString("relay");
            result.validator = node.GetString("validator");
            var balances_array = node.GetNode("balances");
            if (balances_array != null)
            {
                result.balances = new Balance[balances_array.ChildCount];
                for (int i = 0; i < balances_array.ChildCount; i++)
                {

                    result.balances[i] = Balance.FromNode(balances_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.balances = new Balance[0];
            }

            return result;
        }
    }

    public struct ContractParameter
    {
        public string name;
        public string type;
    }

    public struct ContractMethod
    {
        public string name;
        public string returnType;
        public ContractParameter[] parameters;
    }

    public struct Contract
    {
        public string address; //
        public string name; //
        public string script; //
        public ContractMethod[] methods;

        public static Contract FromNode(DataNode node)
        {
            Contract result;

            result.address = node.GetString("address");
            result.name = node.GetString("name");
            result.script = node.GetString("script");

            var methodNode = node.GetNode("methods");
            if (methodNode != null)
            {
                result.methods = new ContractMethod[methodNode.ChildCount];
                for (int i = 0; i < result.methods.Length; i++)
                {
                    var child = methodNode.GetNodeByIndex(i);
                    var method = new ContractMethod();
                    method.name = child.GetString("name");
                    method.returnType = child.GetString("returnType");

                    var paramsNode = child.GetNode("parameters");
                    if (paramsNode != null)
                    {
                        method.parameters = new ContractParameter[paramsNode.ChildCount];
                        for (int j = 0; j < method.parameters.Length; j++)
                        {
                            var temp = paramsNode.GetNodeByIndex(j);
                            var p = new ContractParameter();

                            p.name = temp.GetString("name");
                            p.type = temp.GetString("type");

                            method.parameters[j] = p;
                        }
                    }
                    else
                    {
                        method.parameters = new ContractParameter[0];
                    }

                    result.methods[i] = method;
                }
            }
            else
            {
                result.methods = new ContractMethod[0];
            }

            return result;
        }
    }


    public struct Chain
    {
        public string name; //
        public string address; //
        public string parentAddress; //
        public uint height; //
        public string[] contracts; //

        public static Chain FromNode(DataNode node)
        {
            Chain result;

            result.name = node.GetString("name");
            result.address = node.GetString("address");
            result.parentAddress = node.GetString("parentAddress");
            result.height = node.GetUInt32("height");
            var contracts_array = node.GetNode("contracts");
            if (contracts_array != null)
            {
                result.contracts = new string[contracts_array.ChildCount];
                for (int i = 0; i < contracts_array.ChildCount; i++)
                {

                    result.contracts[i] = contracts_array.GetNodeByIndex(i).AsString();
                }
            }
            else
            {
                result.contracts = new string[0];
            }


            return result;
        }
    }

    public struct Event
    {
        public string address; //
        public string kind; //
        public string contract; //
        public string data; //

        public static Event FromNode(DataNode node)
        {
            Event result;

            result.address = node.GetString("address");
            result.kind = node.GetString("kind");
            result.contract = node.GetString("contract");
            result.data = node.GetString("data");

            return result;
        }
    }

    public struct Transaction
    {
        public string hash; //
        public string chainAddress; //
        public uint timestamp; //
        public int confirmations; //
        public int blockHeight; //
        public string blockHash; //
        public string script; //
        public Event[] events; //
        public string result; //
        public string fee; //

        public static Transaction FromNode(DataNode node)
        {
            Transaction result;

            result.hash = node.GetString("hash");
            result.chainAddress = node.GetString("chainAddress");
            result.timestamp = node.GetUInt32("timestamp");
            result.confirmations = node.GetInt32("confirmations");
            result.blockHeight = node.GetInt32("blockHeight");
            result.blockHash = node.GetString("blockHash");
            result.script = node.GetString("script");
            var events_array = node.GetNode("events");
            if (events_array != null)
            {
                result.events = new Event[events_array.ChildCount];
                for (int i = 0; i < events_array.ChildCount; i++)
                {

                    result.events[i] = Event.FromNode(events_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.events = new Event[0];
            }

            result.result = node.GetString("result");
            result.fee = node.GetString("fee");

            return result;
        }
    }

    public struct AccountTransactions
    {
        public string address; //
        public Transaction[] txs; //

        public static AccountTransactions FromNode(DataNode node)
        {
            AccountTransactions result;

            result.address = node.GetString("address");
            var txs_array = node.GetNode("txs");
            if (txs_array != null)
            {
                result.txs = new Transaction[txs_array.ChildCount];
                for (int i = 0; i < txs_array.ChildCount; i++)
                {

                    result.txs[i] = Transaction.FromNode(txs_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.txs = new Transaction[0];
            }


            return result;
        }
    }

    public struct Block
    {
        public string hash; //
        public string previousHash; //
        public uint timestamp; //
        public uint height; //
        public string chainAddress; //
        public uint protocol; //
        public Transaction[] txs; //
        public string validatorAddress; //
        public string reward; //

        public static Block FromNode(DataNode node)
        {
            Block result;

            result.hash = node.GetString("hash");
            result.previousHash = node.GetString("previousHash");
            result.timestamp = node.GetUInt32("timestamp");
            result.height = node.GetUInt32("height");
            result.chainAddress = node.GetString("chainAddress");
            result.protocol = node.GetUInt32("protocol");
            var txs_array = node.GetNode("txs");
            if (txs_array != null)
            {
                result.txs = new Transaction[txs_array.ChildCount];
                for (int i = 0; i < txs_array.ChildCount; i++)
                {

                    result.txs[i] = Transaction.FromNode(txs_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.txs = new Transaction[0];
            }

            result.validatorAddress = node.GetString("validatorAddress");
            result.reward = node.GetString("reward");

            return result;
        }
    }

    public class TokenPlatform
    {
        public string platform;
        public string hash;

        public static TokenPlatform FromNode(DataNode node)
        {
            var result = new TokenPlatform();

            result.platform = node.GetString("platform");
            result.hash = node.GetString("hash");

            return result;
        }

        public override string ToString()
        {
            return $"Platform {platform}, hash {hash}";
        }
    }

    public class Token
    {
        public string symbol; //
        public bool mainnetToken;
        public string apiSymbol; // API symbols may differ.
        public string name; //
        public int decimals; //
        public string currentSupply; //
        public string maxSupply; //
        public string burnedSupply;
        public string address;
        public string owner;
        public string flags; //
        public string script;
        public TokenPlatform[] external;
        public decimal price;

        public static Token FromNode(DataNode node)
        {
            Token result = new Token();

            result.symbol = node.GetString("symbol");
            result.mainnetToken = true;
            result.name = node.GetString("name");
            result.decimals = node.GetInt32("decimals");
            result.currentSupply = node.GetString("currentSupply");
            result.maxSupply = node.GetString("maxSupply");
            result.burnedSupply = node.GetString("burnedSupply");
            result.address = node.GetString("address");
            result.owner = node.GetString("owner");
            result.flags = node.GetString("flags");
            result.script = node.GetString("script");

            var platforms = new List<TokenPlatform>();
            if (node.HasNode("external"))
            {
                foreach (var platform in node.GetNode("external").Children)
                {
                    platforms.Add(TokenPlatform.FromNode(platform));
                }

                result.external = platforms.ToArray();
            }
            
            result.price = 0;

            return result;
        }

        public bool IsBurnable()
        {
            return flags.Contains(TokenFlags.Burnable.ToString());
        }
        public bool IsFungible()
        {
            return flags.Contains(TokenFlags.Fungible.ToString());
        }
        public bool IsTransferable()
        {
            return flags.Contains(TokenFlags.Transferable.ToString());
        }
        public bool IsSwappable()
        {
            return flags.Contains(TokenFlags.Swappable.ToString());
        }

        public override string ToString()
        {
            var platforms = "";
            if (external != null)
            {
                foreach (var platform in external)
                {
                    platforms += "\t" + platform.ToString() + "\n";
                }
            }
            return $"Symbol {symbol} ({name}), decimals {decimals}, supplies {currentSupply}/{maxSupply}/{burnedSupply}, flags '{flags}', coinGeckoId '{apiSymbol}', mainnetToken '{mainnetToken}'. Platforms:\n{platforms}";
        }
    }

    public struct TokenProperty
    {
        public string Key;
        public string Value;
    }
    public enum TokenStatus
    {
        Active,
        Infused
    }
    public struct TokenData
    {
        public string ID;
        public uint series;
        public uint mint;
        public string chainName;
        public string ownerAddress;
        public string creatorAddress;
        public byte[] ram;
        public byte[] rom;
        public TokenStatus status;
        public IRom parsedRom;
        public TokenProperty[] infusion;
        public List<TokenProperty> properties;

        public static TokenData FromNode(DataNode node, string symbol)
        {
            TokenData result;

            result.ID = node.GetString("ID");
            result.series = node.GetUInt32("series");
            result.mint = node.GetUInt32("mint");
            result.chainName = node.GetString("chainName");
            result.ownerAddress = node.GetString("ownerAddress");
            result.creatorAddress = node.GetString("creatorAddress");
            result.ram = Base16.Decode(node.GetString("ram"));
            result.rom = Base16.Decode(node.GetString("rom"));
            if (!Enum.TryParse(node.GetString("status"), true, out result.status))
                result.status = TokenStatus.Infused;

            // Pasring ROM
            switch (symbol)
            {
                case "CROWN":
                    result.parsedRom = new CrownRom(result.rom, result.ID);
                    break;
                default:
                    result.parsedRom = new CustomRom(result.rom);
                    break;
            }

            // Reading infused assets
            var infusionNode = node.GetNode("infusion");

            if (infusionNode != null)
            {
                var infusion = new List<TokenProperty>();
                foreach (DataNode entry in infusionNode.Children)
                {
                    var property = new TokenProperty();
                    property.Key = entry.GetString("Key");
                    property.Value = entry.GetString("Value");
                    infusion.Add(property);
                }

                result.infusion = infusion.ToArray();
            }
            else
            {
                result.infusion = null;
            }

            // Reading properties
            var propertiesNode = node.GetNode("properties");
            if (propertiesNode != null)
            {
                var properties = new List<TokenProperty>();
                foreach (DataNode entry in propertiesNode.Children)
                {
                    var property = new TokenProperty();
                    property.Key = entry.GetString("Key");
                    property.Value = entry.GetString("Value");
                    properties.Add(property);
                }

                result.properties = properties;
            }
            else
            {
                result.properties = null;
            }

            return result;
        }

        public string GetPropertyValue(string key)
        {
            if (properties != null)
            {
                return properties.Where(x => x.Key.ToUpperInvariant() == key.ToUpperInvariant()).Select(x => x.Value).FirstOrDefault();
            }

            return null;
        }
    }

    public interface IRom
    {
        string GetName();
        string GetDescription();
        DateTime GetDate();
    }
    public class CrownRom : IRom
    {
        public string tokenId;
        public Address staker;
        public Timestamp date;

        public CrownRom(byte[] rom, string tokenId)
        {
            this.tokenId = tokenId;

            using (var stream = new System.IO.MemoryStream(rom))
            {
                using (var reader = new System.IO.BinaryReader(stream))
                {
                    UnserializeData(reader);
                }
            }
        }

        public string GetName() => "CROWN #" + tokenId;
        public string GetDescription() => "";
        public DateTime GetDate() => date;

        private void UnserializeData(System.IO.BinaryReader reader)
        {
            this.staker = reader.ReadAddress();
            this.date = new Timestamp(reader.ReadUInt32());
        }
    }
    public class CustomRom : IRom
    {
        Dictionary<VMObject, VMObject> fields = new Dictionary<VMObject, VMObject>();

        public CustomRom(byte[] romBytes)
        {
            try
            {
                var rom = VMObject.FromBytes(romBytes);
                if (rom.Type == VMType.Struct)
                {
                    fields = (Dictionary<VMObject, VMObject>)rom.Data;
                }
                else
                {
                    Log.Write($"Cannot parse ROM.");
                }
            }
            catch (Exception e)
            {
                Log.Write($"ROM parsing error: {e.ToString()}");
            }
        }

        public string GetName()
        {
            if (fields.TryGetValue(VMObject.FromObject("name"), out var value))
            {
                return value.AsString();
            }
            return "";
        }
        public string GetDescription()
        {
            if (fields.TryGetValue(VMObject.FromObject("description"), out var value))
            {
                return value.AsString();
            }
            return "";
        }
        public DateTime GetDate()
        {
            if (fields.TryGetValue(VMObject.FromObject("created"), out var value))
            {
                return value.AsTimestamp();
            }
            return new DateTime();
        }
    }

    public struct SendRawTx
    {
        public string hash; //
        public string error; //

        public static SendRawTx FromNode(DataNode node)
        {
            SendRawTx result;

            result.hash = node.GetString("hash");
            result.error = node.GetString("error");

            return result;
        }
    }

    public struct Auction
    {
        public string creatorAddress; //
        public string chainAddress; //
        public uint startDate; //
        public uint endDate; //
        public string baseSymbol; //
        public string quoteSymbol; //
        public string tokenId; //
        public string price; //
        public string rom; //
        public string ram; //

        public static Auction FromNode(DataNode node)
        {
            Auction result;

            result.creatorAddress = node.GetString("creatorAddress");
            result.chainAddress = node.GetString("chainAddress");
            result.startDate = node.GetUInt32("startDate");
            result.endDate = node.GetUInt32("endDate");
            result.baseSymbol = node.GetString("baseSymbol");
            result.quoteSymbol = node.GetString("quoteSymbol");
            result.tokenId = node.GetString("tokenId");
            result.price = node.GetString("price");
            result.rom = node.GetString("rom");
            result.ram = node.GetString("ram");

            return result;
        }
    }

    public struct Oracle
    {
        public string url; //
        public string content; //

        public static Oracle FromNode(DataNode node)
        {
            Oracle result;

            result.url = node.GetString("url");
            result.content = node.GetString("content");

            return result;
        }
    }

    public struct Script
    {
        public Event[] events; //
        public string result; //
        public string[] results; //
        public Oracle[] oracles; //

        public static Script FromNode(DataNode node)
        {
            Script result;

            var events_array = node.GetNode("events");
            if (events_array != null)
            {
                result.events = new Event[events_array.ChildCount];
                for (int i = 0; i < events_array.ChildCount; i++)
                {

                    result.events[i] = Event.FromNode(events_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.events = new Event[0];
            }

            result.result = node.GetString("result");
            var oracles_array = node.GetNode("oracles");
            if (oracles_array != null)
            {
                result.oracles = new Oracle[oracles_array.ChildCount];
                for (int i = 0; i < oracles_array.ChildCount; i++)
                {

                    result.oracles[i] = Oracle.FromNode(oracles_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.oracles = new Oracle[0];
            }

            var result_array = node.GetNode("results");
            if (result_array != null)
            {
                result.results= new string[result_array.ChildCount];
                int i = 0;
                foreach (var child in result_array.Children)
                {
                    result.results[i] = child.Value;
                    i++;
                }
            }
            else
            {
                result.results = new string[] { result.result };
            }

            return result;
        }
    }

    public struct Archive
    {
        public string hash; //
        public string name; //
        public uint size; //
        public uint time; //
        public string flags; //
        public IArchiveEncryption encryption; //
        public int blockCount; //
        public string[] metadata; //

        public static Archive FromNode(DataNode node)
        {
            Archive result;

            result.hash = node.GetString("hash");
            result.name = node.GetString("name");
            result.size = node.GetUInt32("size");
            result.time = node.GetUInt32("time");
            result.flags = node.GetString("flags");
            result.encryption = ArchiveExtensions.ReadArchiveEncryption(Base16.Decode(node.GetString("encryption")));
            result.blockCount = node.GetInt32("blockCount");
            var metadata_array = node.GetNode("metadata");
            if (metadata_array != null)
            {
                result.metadata = new string[metadata_array.ChildCount];
                for (int i = 0; i < metadata_array.ChildCount; i++)
                {

                    result.metadata[i] = metadata_array.GetNodeByIndex(i).AsString();
                }
            }
            else
            {
                result.metadata = new string[0];
            }


            return result;
        }
    }

    public struct ABIParameter
    {
        public string name; //
        public string type; //

        public static ABIParameter FromNode(DataNode node)
        {
            ABIParameter result;

            result.name = node.GetString("name");
            result.type = node.GetString("type");

            return result;
        }
    }

    public struct ABIMethod
    {
        public string name; //
        public string returnType; //
        public ABIParameter[] parameters; //

        public static ABIMethod FromNode(DataNode node)
        {
            ABIMethod result;

            result.name = node.GetString("name");
            result.returnType = node.GetString("returnType");
            var parameters_array = node.GetNode("parameters");
            if (parameters_array != null)
            {
                result.parameters = new ABIParameter[parameters_array.ChildCount];
                for (int i = 0; i < parameters_array.ChildCount; i++)
                {

                    result.parameters[i] = ABIParameter.FromNode(parameters_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.parameters = new ABIParameter[0];
            }


            return result;
        }
    }

    public struct ABIContract
    {
        public string name; //
        public ABIMethod[] methods; //

        public static ABIContract FromNode(DataNode node)
        {
            ABIContract result;

            result.name = node.GetString("name");
            var methods_array = node.GetNode("methods");
            if (methods_array != null)
            {
                result.methods = new ABIMethod[methods_array.ChildCount];
                for (int i = 0; i < methods_array.ChildCount; i++)
                {

                    result.methods[i] = ABIMethod.FromNode(methods_array.GetNodeByIndex(i));

                }
            }
            else
            {
                result.methods = new ABIMethod[0];
            }


            return result;
        }
    }

    public struct Channel
    {
        public string creatorAddress; //
        public string targetAddress; //
        public string name; //
        public string chain; //
        public uint creationTime; //
        public string symbol; //
        public string fee; //
        public string balance; //
        public Boolean active; //
        public int index; //

        public static Channel FromNode(DataNode node)
        {
            Channel result;

            result.creatorAddress = node.GetString("creatorAddress");
            result.targetAddress = node.GetString("targetAddress");
            result.name = node.GetString("name");
            result.chain = node.GetString("chain");
            result.creationTime = node.GetUInt32("creationTime");
            result.symbol = node.GetString("symbol");
            result.fee = node.GetString("fee");
            result.balance = node.GetString("balance");
            result.active = node.GetBoolean("active");
            result.index = node.GetInt32("index");

            return result;
        }
    }

    public struct Receipt
    {
        public string nexus; //
        public string channel; //
        public string index; //
        public uint timestamp; //
        public string sender; //
        public string receiver; //
        public string script; //

        public static Receipt FromNode(DataNode node)
        {
            Receipt result;

            result.nexus = node.GetString("nexus");
            result.channel = node.GetString("channel");
            result.index = node.GetString("index");
            result.timestamp = node.GetUInt32("timestamp");
            result.sender = node.GetString("sender");
            result.receiver = node.GetString("receiver");
            result.script = node.GetString("script");

            return result;
        }
    }

    public struct Peer
    {
        public string url; //
        public string flags; //
        public string fee; //
        public uint pow; //

        public static Peer FromNode(DataNode node)
        {
            Peer result;

            result.url = node.GetString("url");
            result.flags = node.GetString("flags");
            result.fee = node.GetString("fee");
            result.pow = node.GetUInt32("pow");

            return result;
        }
    }

    public struct Validator
    {
        public string address; //
        public string type; //

        public static Validator FromNode(DataNode node)
        {
            Validator result;

            result.address = node.GetString("address");
            result.type = node.GetString("type");

            return result;
        }
    }

    public class PhantasmaAPI
    {
        public readonly string Host;

        public PhantasmaAPI(string host)
        {
            this.Host = host;
        }


        //Returns the account name and balance of given address.
        public async Task<Account> GetAccount(string addressText)
        {
            var node = await WebClient.RPCRequestAsync(Host, "getAccount", WebClient.DefaultTimeout, 0, addressText);

            return Account.FromNode(node);
        }

        public IEnumerator GetContract(string contractName, Action<Contract> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getContract", WebClient.DefaultTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Contract.FromNode(node);
                callback(result);
            }, DomainSettings.RootChainName, contractName);
        }


        //Returns the address that owns a given name.
        public IEnumerator LookUpName(string name, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "lookUpName", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = node.Value;
                callback(result);
            }, name);
        }


        //Returns the height of a chain.
        public IEnumerator GetBlockHeight(string chainInput, Action<int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getBlockHeight", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = int.Parse(node.Value);
                callback(result);
            }, chainInput);
        }


        //Returns the number of transactions of given block hash or error if given hash is invalid or is not found.
        public IEnumerator GetBlockTransactionCountByHash(string blockHash, Action<int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getBlockTransactionCountByHash", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = int.Parse(node.Value);
                callback(result);
            }, blockHash);
        }


        //Returns information about a block by hash.
        public IEnumerator GetBlockByHash(string blockHash, Action<Block> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getBlockByHash", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Block.FromNode(node);
                callback(result);
            }, blockHash);
        }


        //Returns a serialized string, containing information about a block by hash.
        public IEnumerator GetRawBlockByHash(string blockHash, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getRawBlockByHash", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = node.Value;
                callback(result);
            }, blockHash);
        }


        //Returns information about a block by height and chain.
        public IEnumerator GetBlockByHeight(string chainInput, uint height, Action<Block> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getBlockByHeight", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Block.FromNode(node);
                callback(result);
            }, chainInput, height);
        }


        //Returns a serialized string, in hex format, containing information about a block by height and chain.
        public IEnumerator GetRawBlockByHeight(string chainInput, uint height, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getRawBlockByHeight", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = node.Value;
                callback(result);
            }, chainInput, height);
        }


        //Returns the information about a transaction requested by a block hash and transaction index.
        public IEnumerator GetTransactionByBlockHashAndIndex(string blockHash, int index, Action<Transaction> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getTransactionByBlockHashAndIndex", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Transaction.FromNode(node);
                callback(result);
            }, blockHash, index);
        }


        //Returns last X transactions of given address.
        //This api call is paginated, multiple calls might be required to obtain a complete result 
        public IEnumerator GetAddressTransactions(string addressText, uint page, uint pageSize, Action<AccountTransactions, int, int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getAddressTransactions", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var currentPage = node.GetInt32("page");
                var totalPages = node.GetInt32("totalPages");
                node = node.GetNode("result");
                var result = AccountTransactions.FromNode(node);
                callback(result, currentPage, totalPages);
            }, addressText, page, pageSize);
        }

        public IEnumerator GetAllAddressTransactions(string addressText, Action<AccountTransactions> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getAddressTransactions", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                node = node.GetNode("result");
                var result = AccountTransactions.FromNode(node);
                callback(result);
            }, addressText);
        }
        //Get number of transactions in a specific address and chain
        public IEnumerator GetAddressTransactionCount(string addressText, string chainInput, Action<int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getAddressTransactionCount", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = int.Parse(node.Value);
                callback(result);
            }, addressText, chainInput);
        }


        //Allows to broadcast a signed operation on the network, but it&apos;s required to build it manually.
        public IEnumerator SendRawTransaction(string txData, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "sendRawTransaction", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = node.Value;
                callback(result);
            }, txData);
        }


        //Allows to invoke script based on network state, without state changes.
        public IEnumerator InvokeRawScript(string chainInput, string scriptData, Action<Script> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "invokeRawScript", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Script.FromNode(node);
                callback(result);
            }, chainInput, scriptData);
        }


        //Returns information about a transaction by hash.
        public IEnumerator GetTransaction(string hashText, Action<Transaction> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getTransaction", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Transaction.FromNode(node);
                callback(result);
            }, hashText);
        }


        //Removes a pending transaction from the mempool.
        public IEnumerator CancelTransaction(string hashText, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "cancelTransaction", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = node.Value;
                callback(result);
            }, hashText);
        }


        //Returns an array of all chains deployed in Phantasma.
        public IEnumerator GetChains(Action<Chain[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getChains", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = new Chain[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Chain.FromNode(child);
                }
                callback(result);
            });
        }


        //Returns an array of tokens deployed in Phantasma.
        public IEnumerator GetTokens(Action<Token[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getTokens", 5, 1, errorHandlingCallback, (node) =>
            {
                var result = new Token[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Token.FromNode(child);
                }
                callback(result);
            });
        }


        //Returns info about a specific token deployed in Phantasma.
        public IEnumerator GetToken(string symbol, Action<Token> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getToken", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Token.FromNode(node);
                callback(result);
            }, symbol);
        }


        private int tokensLoadedSimultaneously = 0;

        //Returns data of a non-fungible token, in hexadecimal format.
        public IEnumerator GetNFT(string symbol, string IDtext, Action<DataNode> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            while (tokensLoadedSimultaneously > 5)
            {
                yield return null;
            }
            tokensLoadedSimultaneously++;

            yield return WebClient.RPCRequest(Host, "getNFT", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                callback(node);
            }, symbol, IDtext, true);

            tokensLoadedSimultaneously--;
        }


        //Returns last X transactions of given token.
        //This api call is paginated, multiple calls might be required to obtain a complete result 
        public IEnumerator GetTokenTransfers(string tokenSymbol, uint page, uint pageSize, Action<Transaction[], int, int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getTokenTransfers", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var currentPage = node.GetInt32("page");
                var totalPages = node.GetInt32("totalPages");
                node = node.GetNode("result");
                var result = new Transaction[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Transaction.FromNode(child);
                }
                callback(result, currentPage, totalPages);
            }, tokenSymbol, page, pageSize);
        }


        //Returns the number of transaction of a given token.
        public IEnumerator GetTokenTransferCount(string tokenSymbol, Action<int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getTokenTransferCount", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = int.Parse(node.Value);
                callback(result);
            }, tokenSymbol);
        }


        //Returns the balance for a specific token and chain, given an address.
        public IEnumerator GetTokenBalance(string addressText, string tokenSymbol, string chainInput, Action<Balance> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getTokenBalance", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Balance.FromNode(node);
                callback(result);
            }, addressText, tokenSymbol, chainInput);
        }


        //Returns the number of active auctions.
        public IEnumerator GetAuctionsCount(string chainAddressOrName, string symbol, Action<int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getAuctionsCount", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = int.Parse(node.Value);
                callback(result);
            }, chainAddressOrName, symbol);
        }


        //Returns the auctions available in the market.
        //This api call is paginated, multiple calls might be required to obtain a complete result 
        public IEnumerator GetAuctions(string chainAddressOrName, string symbol, uint page, uint pageSize, Action<Auction[], int, int> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getAuctions", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var currentPage = node.GetInt32("page");
                var totalPages = node.GetInt32("totalPages");
                node = node.GetNode("result");
                var result = new Auction[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Auction.FromNode(child);
                }
                callback(result, currentPage, totalPages);
            }, chainAddressOrName, symbol, page, pageSize);
        }


        //Returns the auction for a specific token.
        public IEnumerator GetAuction(string chainAddressOrName, string symbol, string IDtext, Action<Auction> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getAuction", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Auction.FromNode(node);
                callback(result);
            }, chainAddressOrName, symbol, IDtext);
        }


        //Returns info about a specific archive.
        public IEnumerator GetArchive(string hashText, Action<Archive> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getArchive", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Archive.FromNode(node);
                callback(result);
            }, hashText);
        }


        //Writes the contents of an incomplete archive.
        public IEnumerator WriteArchive(string hashText, int blockIndex, byte[] blockContent, Action<Boolean> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "writeArchive", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Boolean.Parse(node.Value);
                callback(result);
            }, hashText, blockIndex, Convert.ToBase64String(blockContent));
        }

        // Reads the contents of an archive block.
        public IEnumerator ReadArchive(string hashText, int blockIndex, Action<byte[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "readArchive", WebClient.NoTimeout, 5, errorHandlingCallback, (node) =>
            {
                var result = Convert.FromBase64String(node.Value);
                callback(result);
            }, hashText, blockIndex);
        }


        //Returns the ABI interface of specific contract.
        public IEnumerator GetABI(string chainAddressOrName, string contractName, Action<ABIContract> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getABI", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = ABIContract.FromNode(node);
                callback(result);
            }, chainAddressOrName, contractName);
        }


        //Returns list of known peers.
        public IEnumerator GetPeers(Action<Peer[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getPeers", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = new Peer[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Peer.FromNode(child);
                }
                callback(result);
            });
        }


        //Writes a message to the relay network.
        public IEnumerator RelaySend(string receiptHex, Action<Boolean> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "relaySend", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = Boolean.Parse(node.Value);
                callback(result);
            }, receiptHex);
        }


        //Receives messages from the relay network.
        public IEnumerator RelayReceive(string accountInput, Action<Receipt[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "relayReceive", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = new Receipt[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Receipt.FromNode(child);
                }
                callback(result);
            }, accountInput);
        }


        //Reads pending messages from the relay network.
        public IEnumerator GetEvents(string accountInput, Action<Event[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getEvents", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = new Event[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Event.FromNode(child);
                }
                callback(result);
            }, accountInput);
        }


        //Returns platform swaps for a specific address.
        public IEnumerator SettleSwap(string sourcePlatform, string destPlatform, string hashText, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "settleSwap", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = node.Value;
                callback(result);
            }, sourcePlatform, destPlatform, hashText);
        }


        //Returns platform swaps for a specific address.
        public IEnumerator GetSwapsForAddress(string accountInput, string platform, Action<Swap[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getSwapsForAddress", WebClient.NoTimeout, 0, (error, msg) =>
            {
                errorHandlingCallback(error, msg);
            }, (node) =>
            {
                var result = new Swap[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Swap.FromNode(child);
                }
                callback(result);
            }, accountInput, platform);
        }



        //Returns an array of available interop platforms.
        public IEnumerator GetPlatforms(Action<Platform[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getPlatforms", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = new Platform[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Platform.FromNode(child);
                }
                callback(result);
            });
        }


        //Returns an array of available validators.
        public IEnumerator GetValidators(Action<Validator[]> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            yield return WebClient.RPCRequest(Host, "getValidators", WebClient.NoTimeout, 0, errorHandlingCallback, (node) =>
            {
                var result = new Validator[node.ChildCount];
                for (int i = 0; i < result.Length; i++)
                {
                    var child = node.GetNodeByIndex(i);
                    result[i] = Validator.FromNode(child);
                }
                callback(result);
            });
        }


        public IEnumerator SignAndSendTransaction(PhantasmaKeys keys, string nexus, byte[] script, string chain, BigInteger gasPrice, BigInteger gasLimit, ProofOfWork PoW, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            return SignAndSendTransactionWithPayload(keys, null, nexus, script, chain, gasPrice, gasLimit, new byte[0], PoW, callback, errorHandlingCallback);
        }

        public IEnumerator SignAndSendTransactionWithPayload(PhantasmaKeys keys, string nexus, byte[] script, string chain, BigInteger gasPrice, BigInteger gasLimit, string payload, ProofOfWork PoW, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null)
        {
            return SignAndSendTransactionWithPayload(keys, null, nexus, script, chain, gasPrice, gasLimit, Encoding.UTF8.GetBytes(payload), PoW, callback, errorHandlingCallback);
        }

        public IEnumerator SignAndSendTransactionWithPayload(PhantasmaKeys keys, IKeyPair otherKeys, string nexus, byte[] script, string chain, BigInteger gasPrice, BigInteger gasLimit, byte[] payload, ProofOfWork PoW, Action<string> callback, Action<EPHANTASMA_SDK_ERROR_TYPE, string> errorHandlingCallback = null, Func<byte[], byte[], byte[], byte[]> customSignFunction = null)
        {
            Log.Write("Sending transaction...");

            var tx = new Phantasma.Core.Domain.Transaction(nexus, chain, 0L, script, keys.Address, keys.Address, Address.Null, gasPrice, gasLimit, DateTime.UtcNow + TimeSpan.FromMinutes(20), payload);

            if (PoW != ProofOfWork.None)
            {
                tx.Mine(PoW);
            }

            tx.Sign(keys, null);
            if (otherKeys != null)
            {
                tx.Sign(otherKeys, customSignFunction);
            }

            yield return SendRawTransaction(Base16.Encode(tx.ToByteArray(true)), callback, errorHandlingCallback);
        }

        public static bool IsValidPrivateKey(string key)
        {
            return (key.StartsWith("L", false, CultureInfo.InvariantCulture) ||
                    key.StartsWith("K", false, CultureInfo.InvariantCulture)) && key.Length == 52;
        }

        public static bool IsValidAddress(string address)
        {
            return address.StartsWith("P", false, CultureInfo.InvariantCulture) && address.Length == 45;
        }
    }
}