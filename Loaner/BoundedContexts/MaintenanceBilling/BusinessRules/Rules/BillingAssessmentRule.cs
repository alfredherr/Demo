﻿using System;
using System.Collections.Generic;
using System.Linq;
using Loaner.BoundedContexts.MaintenanceBilling.Aggregates.Models;
using Loaner.BoundedContexts.MaintenanceBilling.BusinessRules.Handler;
using Loaner.BoundedContexts.MaintenanceBilling.DomainCommands;
using Loaner.BoundedContexts.MaintenanceBilling.DomainEvents;
using Loaner.BoundedContexts.MaintenanceBilling.DomainModels;

namespace Loaner.BoundedContexts.MaintenanceBilling.BusinessRules.Rules
{
    public class BillingAssessmentRule : IAccountBusinessRule
    {
        private readonly List<IDomainEvent> _eventsGenerated = new List<IDomainEvent>();
        private string _detailsGenerated;
        private IAccountBusinessRule _accountBusinessRuleImplementation;

        public BillingAssessmentRule()
        {
            
        }

        public BillingAssessmentRule((string Command, Dictionary<string, object> Parameters) commandState)
        {
            CommandState = commandState;
        }

        public BillingAssessmentRule(AccountState accountState)
        {
            AccountState = accountState;
        }

        private AccountState AccountState { get; set; }
        private (string Command, Dictionary<string, object> Parameters) CommandState { get; set; }

        public bool Success { get; private set; }

        /* Rule logic goes here. */
        public void RunRule(IDomainCommand command)
        {    switch (command)
            {
                case BillingAssessment billing:
                    RunRule(billing);
                    break;
                default:
                    throw new NotImplementedException();
            }
           
        }
        public void RunRule(BillingAssessment command)
        {
            //User CommandState to get list of passed in options. 
            // and COMMAND to merge the specifics of the command

            foreach (var billLine in command.LineItems)
            {
                _eventsGenerated.Add(new ObligationAssessedConcept(
                        AccountState.Obligations.FirstOrDefault(x => x.Value.Status == ObligationStatus.Active)
                            .Key,
                        billLine.Item
                    )
                );
                _detailsGenerated = "THIS WORKED";
                Success = true;
            }
        }

        public void SetAccountState(AccountState state)
        {
            AccountState = state;
        }

        public void SetCallingCommandState((string Command, Dictionary<string, object> Parameters) commandState)
        {
            CommandState = commandState;
        }

       

        public string GetResultDetails()
        {
            return _detailsGenerated;
        }

        public List<IDomainEvent> GetGeneratedEvents()
        {
            return _eventsGenerated;
        }

        public AccountState GetGeneratedState()
        {
            return AccountState;
        }

        public bool RuleAppliedSuccessfuly()
        {
            return Success;
        }
    }
}