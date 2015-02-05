/*
 * Created by SharpDevelop.
 * User: bcrawford
 * Date: 9/17/2014
 * Time: 2:57 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using NLog;
using NLog.Targets;
using NLog.Config;

namespace Logger
{
  /// <summary>
  /// Description of MyClass.
  /// </summary>
  public sealed class LoggerAsync
  {
    private static LoggerAsync instance;
    
    public static LoggerAsync InstanceOf
    {
      get {
        if (instance == null) {
          lock (sync) {
            if (instance == null) {
              instance = new LoggerAsync();
            }
          }
        }
        
        return instance;
      }
    }
    
    private static object sync = new Object();
    
    private LoggerAsync()
    {
      // from Nlog documentation
      //step 1: config object
      var config = new LoggingConfiguration();
      
      //step 2: targets
      var generalFileTarget = new FileTarget();
      
      generalFileTarget.ConcurrentWrites = true;
      //generalFileTarget.ArchiveAboveSize = 600L * 1000L * 100L; //60 Mb
      generalFileTarget.ArchiveFileName = "${basedir}/archive_logs/general-{#}.log";
      generalFileTarget.ArchiveEvery = FileArchivePeriod.Day;
      generalFileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
      
      config.AddTarget("general", generalFileTarget);
      
      var exchangeFileTarget = new FileTarget();
      
      exchangeFileTarget.ArchiveFileName = "${basedir}/archive_logs/exchange-{#}.log";
      //exchangeFileTarget.ArchiveAboveSize = 600L * 1000L * 100L; //60 Mb
      exchangeFileTarget.ConcurrentWrites = true;
      exchangeFileTarget.ArchiveEvery = FileArchivePeriod.Day;
      exchangeFileTarget.ArchiveNumbering = ArchiveNumberingMode.Date;
      
      config.AddTarget("exchange", exchangeFileTarget);
      
      //step 3: set target properties
      generalFileTarget.FileName = "${basedir}/logs/general.log";
      generalFileTarget.Layout = "${longdate}|${level}|${logger}|${message}|${exception:format=ToString,StackTrace}${newline}";
      
      exchangeFileTarget.FileName = "${basedir}/logs/exchange.log";
      exchangeFileTarget.Layout = "${longdate}|${level}|${logger}|${message}|${exception:format=ToString,StackTrace}${newline}";
      
      //step 4: set rules
      var generalRule = new LoggingRule("RfiCoder.*", generalFileTarget);
      
      generalRule.EnableLoggingForLevel(LogLevel.Trace);
      
      config.LoggingRules.Add(generalRule);
      
      var exchangeRule = new LoggingRule("Microsoft.Exchange.WebServices.*", NLog.LogLevel.Trace, exchangeFileTarget);
      
      config.LoggingRules.Add(exchangeRule);
      
      //step 5: activate
      LogManager.Configuration = config;
      
      SimpleConfigurator.ConfigureForTargetLogging(exchangeFileTarget, LogLevel.Trace);
      
      SimpleConfigurator.ConfigureForTargetLogging(generalFileTarget, LogLevel.Trace);
      
      generalLogger = NLog.LogManager.GetLogger("general");
      
      exchangeLogger = NLog.LogManager.GetLogger("exchange");
      
    }
    private NLog.Logger generalLogger;
    
    private NLog.Logger exchangeLogger;
    
    public NLog.Logger GeneralLogger
    {
      get { return generalLogger; }
    }
    
    public NLog.Logger ExchangeLogger
    {
      get { return exchangeLogger; }
    }
  }
}