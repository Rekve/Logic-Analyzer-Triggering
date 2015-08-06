﻿
using ABB.Robotics.Paint.RobView.Database.SignalLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LogialAnalyzerTrigger.ViewModels
{
    class TriggerForkedStatementData : BindableBase
    {
        private Statement.Statement _statement;
        private List<TriggerStateThenOperations> _thenOperations;
        private IEnumerable<Signal> _avalibleSignals;
        private string _thenOperationString;

        private readonly DelegateCommand<string> _addStatementCommand;
        private readonly DelegateCommand<string> _addThenOperationsCommand;

        public TriggerForkedStatementData(IEnumerable<Signal> avalibleSignals)
        {
            _avalibleSignals = avalibleSignals;
            _thenOperationsString = "";
            _statement = new Statement.Equals(new Signal { Name = "A1" }, 0);

            _addStatementCommand = new DelegateCommand<string>(
                  (s) => { AddStatement(); }, //Execute
                  (s) => { return true; } //CanExecute
                  );

            _addThenOperationsCommand = new DelegateCommand<string>(
                  (s) => { AddThenOperation(); }, //Execute
                  (s) => { return true; } //CanExecute
                  );
        }

        public DelegateCommand<string> AddStatementCommand
        {
            get { return _addStatementCommand; }
        }

        public DelegateCommand<string> AddThenOperationsCommand
        {
            get { return _addThenOperationsCommand; }
        }

        public string ThenOperationsString
        {
            get { return _thenOperationString; }
            set { SetProperty(ref _thenOperationString, value); }
        }

        public Statement.Statement Statement
        {
            get { return _statement; }
            set { SetProperty(ref _statement, value); }
        }

        public List<TriggerStateThenOperations> ThenOperations
        {
            get { return _thenOperations; }
            set { SetProperty(ref _thenOperations, value); }
        }

        private void AddThenOperation()
        {
            var thenOperationModelList = new List<ThenOperationModel>();
            var thenOperationVm = new ThenOperationsVM(thenOperationModelList);
            var thenOperationView = new Views.ThenOperationsWindow();
            thenOperationView.DataContext = thenOperationVm;
            thenOperationView.ShowDialog();


            var thenOperationList = new List<TriggerStateThenOperations>();
            var thenOperationString = "";
            if (thenOperationVm.IS_SAVED)
            {
                foreach (ThenOperationModel thenOperartionModel in thenOperationModelList)
                {
                    thenOperationList.Add(thenOperartionModel.ThenOperation);
                    
                }
                _thenOperations = thenOperationList;
            }
        }

        private void AddStatement()
        {
            var statementDataList = new List<StatementData>();

            var statementVm = new StatementVm3(statementDataList, _avalibleSignals);
            var statementView = new Views.StatementWindow3();
            statementView.DataContext = statementVm;
            statementView.ShowDialog();

            if (statementVm.VALID_DATA_FLAG)
            {
                if (statementDataList.Count > 1)
                {
                    var statement = new Statement.StatementCollection();
                    foreach (StatementData statementdata in statementDataList)
                    {
                        switch (statementdata.AlgebraOperator)
                        {
                            case "Or":
                                statement.AddStatement(CreateStatement(statementdata), LogialAnalyzerTrigger.Statement.LogicalOperators.Or);
                                break;
                            case "And":
                                statement.AddStatement(CreateStatement(statementdata), LogialAnalyzerTrigger.Statement.LogicalOperators.And);
                                break;
                            default:
                                statement.AddStatement(CreateStatement(statementdata), LogialAnalyzerTrigger.Statement.LogicalOperators.First);
                                break;
                        }

                    }
                    Statement = statement;
                }
                else
                {
                    Statement = CreateStatement(statementDataList.First());
                }
            }
            else
            {
                MessageBoxResult msgBox = MessageBox.Show("Statement creaton canceled");
            }

        }

        private Statement.Statement CreateStatement(StatementData statementValues)
        {
            Statement.Statement statement;

            var signal = _avalibleSignals.First(x => x.Name.Equals(statementValues.Signal));

            if (_avalibleSignals.Any(x => x.Name.Equals(statementValues.SignalOrValue)))
            {
                var signal2 = _avalibleSignals.First(x => x.Name.Equals(statementValues.SignalOrValue));

                switch (statementValues.ExpressionValue)
                {
                    case "Equals":
                        statement = new Statement.Equals(signal, signal2);
                        break;
                    case "Lagrer Then":
                        statement = new Statement.LargerThen(signal, signal2);
                        break;
                    case "Less Then":
                        statement = new Statement.LessThen(signal, signal2);
                        break;
                    case "Rising Edge":
                        statement = new Statement.RisingEdge(signal, signal2);
                        break;
                    case "Falling Edge":
                        statement = new Statement.FallingEdge(signal, signal2);
                        break;
                    default:
                        throw new Exception();
                }

            }
            else
            {
                double value;
                if (double.TryParse(statementValues.SignalOrValue, out value))
                {
                    switch (statementValues.ExpressionValue)
                    {
                        case "Equals":
                            statement = new Statement.Equals(signal, value);
                            break;
                        case "Larger Then":
                            statement = new Statement.LargerThen(signal, value);
                            break;
                        case "Less Then":
                            statement = new Statement.LessThen(signal, value);
                            break;
                        case "Rising Edge":
                            statement = new Statement.RisingEdge(signal, value);
                            break;
                        case "Falling Edge":
                            statement = new Statement.FallingEdge(signal, value);
                            break;
                        default:
                            throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            return statement;
        }

    }
}
