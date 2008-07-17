using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core.Imex;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using System.Runtime.Serialization;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Healthcare.Imex
{
    [ExtensionOf(typeof(XmlDataImexExtensionPoint))]
    [ImexDataClass("Worklist")]
    public class WorklistImex : XmlEntityImex<Worklist, WorklistImex.WorklistData>
    {
        #region Data Contracts

        [DataContract]
        public class WorklistData
        {
            [DataContract]
            public class StaffSubscriberData
            {
                [DataMember]
                public string StaffId;
            }

            [DataContract]
            public class GroupSubscriberData
            {
                [DataMember]
                public string StaffGroupName;
            }

            [DataContract]
            public abstract class FilterData
            {
                [DataMember]
                public bool Enabled;
            }

            [DataContract]
            public class SingleValuedFilterData<TValue> : FilterData
            {
                [DataMember]
                public TValue Value;
            }

            [DataContract]
            public class MultiValuedFilterData<TValue> : FilterData
            {
                [DataMember]
                public List<TValue> Values;
            }

            [DataContract]
            public class ProcedureTypeGroupData
            {
                [DataMember]
                public string Name;

                [DataMember]
                public string Class;
            }

			[DataContract]
			public class LocationData
			{
				public LocationData()
				{
				}

				public LocationData(string id)
				{
					this.Id = id;
				}

				[DataMember]
				public string Id;
			}

            [DataContract]
            public class EnumValueData
            {
                public EnumValueData()
                {

                }

                public EnumValueData(string code)
                {
                    this.Code = code;
                }

                [DataMember]
                public string Code;
            }

            [DataContract]
            public class FacilitiesFilterData : MultiValuedFilterData<EnumValueData>
            {
                public FacilitiesFilterData()
                {

                }

                public FacilitiesFilterData(MultiValuedFilterData<EnumValueData> x)
                {
                    this.Enabled = x.Enabled;
                    this.Values = x.Values;
                }

                [DataMember]
                public bool IncludeWorkingFacility;
            }

			[DataContract]
			public class TimeRangeData
			{
				public TimeRangeData()
				{
				}

				public TimeRangeData(WorklistTimeRange tr)
				{
					this.Start = tr.Start == null ? null : new TimePointData(tr.Start);
					this.End = tr.End == null ? null : new TimePointData(tr.End);
				}

				public WorklistTimeRange CreateTimeRange()
				{
					return new WorklistTimeRange(
						this.Start == null ? null : this.Start.CreateTimePoint(),
						this.End == null ? null : this.End.CreateTimePoint());
				}

				[DataMember]
				public TimePointData Start;

				[DataMember]
				public TimePointData End;
			}

			[DataContract]
			public class TimePointData
			{
				public TimePointData()
				{
				}

				public TimePointData(WorklistTimePoint tp)
				{
					this.FixedValue = tp.IsFixed ? tp.FixedValue : null;
					this.RelativeValue = tp.IsRelative ? tp.RelativeValue.Value.Ticks.ToString() : null;
					this.Resolution = tp.Resolution;
				}

				public WorklistTimePoint CreateTimePoint()
				{
					if(this.FixedValue != null)
						return new WorklistTimePoint(this.FixedValue.Value, this.Resolution);
					else if(this.RelativeValue != null)
						return new WorklistTimePoint(TimeSpan.FromTicks(Int64.Parse(this.RelativeValue)), this.Resolution);
					else 
						return null;
				}

				[DataMember]
				public DateTime? FixedValue;

				[DataMember]
				public string RelativeValue;

				[DataMember]
				public long Resolution;
			}

            [DataContract]
            public class FiltersData
            {
                public FiltersData()
                {
                    this.ProcedureTypeGroups = new MultiValuedFilterData<ProcedureTypeGroupData>();
                    this.Facilities = new FacilitiesFilterData();
                    this.OrderPriorities = new MultiValuedFilterData<EnumValueData>();
                    this.PatientClasses = new MultiValuedFilterData<EnumValueData>();
					this.PatientLocations = new MultiValuedFilterData<LocationData>();
                    this.Portable = new SingleValuedFilterData<bool>();
                	this.TimeWindow = new SingleValuedFilterData<TimeRangeData>();
                }

                [DataMember]
                public MultiValuedFilterData<ProcedureTypeGroupData> ProcedureTypeGroups;

                [DataMember]
                public FacilitiesFilterData Facilities;

                [DataMember]
                public MultiValuedFilterData<EnumValueData> OrderPriorities;

                [DataMember]
                public MultiValuedFilterData<EnumValueData> PatientClasses;

				[DataMember]
				public MultiValuedFilterData<LocationData> PatientLocations;

				[DataMember]
                public SingleValuedFilterData<bool> Portable;

				[DataMember]
				public SingleValuedFilterData<TimeRangeData> TimeWindow;
            }

            public WorklistData()
            {
                this.Filters = new FiltersData();
            }

            [DataMember]
            public string Name;

            [DataMember]
            public string Class;

            [DataMember]
            public string Description;

            [DataMember]
            public FiltersData Filters;

            [DataMember]
            public List<StaffSubscriberData> StaffSubscribers;

            [DataMember]
            public List<GroupSubscriberData> GroupSubscribers;
        }


        #endregion

        #region Overrides

        protected override IList<Worklist> GetItemsForExport(IReadContext context, int firstRow, int maxRows)
        {
            WorklistSearchCriteria where = new WorklistSearchCriteria();
            where.Name.SortAsc(0);
            where.FullClassName.SortAsc(1);
            return context.GetBroker<IWorklistBroker>().Find(where, new SearchResultPage(firstRow, maxRows));
        }

        protected override WorklistData Export(Worklist worklist, IReadContext context)
        {
            WorklistData data = new WorklistData();
            data.Class = worklist.GetClass().FullName;
            data.Name = worklist.Name;
            data.Description = worklist.Description;

            data.StaffSubscribers = CollectionUtils.Map<Staff, WorklistData.StaffSubscriberData>(
                worklist.StaffSubscribers,
                delegate(Staff staff)
                {
                    WorklistData.StaffSubscriberData s = new WorklistData.StaffSubscriberData();
                    s.StaffId = staff.Id;
                    return s;
                });

            data.GroupSubscribers = CollectionUtils.Map<StaffGroup, WorklistData.GroupSubscriberData>(
                worklist.GroupSubscribers,
                delegate(StaffGroup group)
                {
                    WorklistData.GroupSubscriberData s = new WorklistData.GroupSubscriberData();
                    s.StaffGroupName = group.Name;
                    return s;
                });

            ExportFilter(
                worklist.ProcedureTypeGroupFilter,
                data.Filters.ProcedureTypeGroups,
                delegate(ProcedureTypeGroup group)
                {
                    WorklistData.ProcedureTypeGroupData s = new WorklistData.ProcedureTypeGroupData();
                    s.Class = group.GetClass().FullName;
                    s.Name = group.Name;
                    return s;
                });

            data.Filters.Facilities.IncludeWorkingFacility = worklist.FacilityFilter.IncludeWorkingFacility;
            ExportFilter(
                worklist.FacilityFilter,
                data.Filters.Facilities,
                delegate(Facility item) { return new WorklistData.EnumValueData(item.Code); });



            ExportFilter(worklist.OrderPriorityFilter, data.Filters.OrderPriorities,
                delegate(OrderPriorityEnum item) { return new WorklistData.EnumValueData(item.Code); });
            ExportFilter(worklist.PatientClassFilter, data.Filters.PatientClasses,
                delegate(PatientClassEnum item) { return new WorklistData.EnumValueData(item.Code); });
			ExportFilter(worklist.PatientLocationFilter, data.Filters.PatientLocations,
				delegate(Location item) { return new WorklistData.LocationData(item.Id); });

            data.Filters.Portable.Enabled = worklist.PortableFilter.IsEnabled;
            data.Filters.Portable.Value = worklist.PortableFilter.Value;

			//Bug #2429: don't forget to include the time filter
			data.Filters.TimeWindow.Enabled = worklist.TimeFilter.IsEnabled;
        	data.Filters.TimeWindow.Value = worklist.TimeFilter.Value == null ? null : 
				new WorklistData.TimeRangeData(worklist.TimeFilter.Value);

            return data;
        }

        protected override void Import(WorklistData data, IUpdateContext context)
        {
            Worklist worklist = LoadOrCreateWorklist(data.Name, data.Class, context);
            worklist.Description = data.Description;

            if (data.StaffSubscribers != null)
            {
                foreach (WorklistData.StaffSubscriberData s in data.StaffSubscribers)
                {
                    StaffSearchCriteria criteria = new StaffSearchCriteria();
                    criteria.Id.EqualTo(s.StaffId);

                    IList<Staff> staff = context.GetBroker<IStaffBroker>().Find(criteria);
                    if(staff.Count == 1)
                        worklist.StaffSubscribers.Add(CollectionUtils.FirstElement(staff));
                }
            }

            if (data.GroupSubscribers != null)
            {
                foreach (WorklistData.GroupSubscriberData s in data.GroupSubscribers)
                {
                    StaffGroupSearchCriteria criteria = new StaffGroupSearchCriteria();
                    criteria.Name.EqualTo(s.StaffGroupName);

                    IList<StaffGroup> groups = context.GetBroker<IStaffGroupBroker>().Find(criteria);
                    if (groups.Count == 1)
                        worklist.GroupSubscribers.Add(CollectionUtils.FirstElement(groups));
                }
            }

            ImportFilter(
                worklist.ProcedureTypeGroupFilter,
                data.Filters.ProcedureTypeGroups,
                delegate(WorklistData.ProcedureTypeGroupData s)
                {
                    ProcedureTypeGroupSearchCriteria criteria = new ProcedureTypeGroupSearchCriteria();
                    criteria.Name.EqualTo(s.Name);

                    IProcedureTypeGroupBroker broker = context.GetBroker<IProcedureTypeGroupBroker>();
                    return CollectionUtils.FirstElement(broker.Find(criteria, ProcedureTypeGroup.GetSubClass(s.Class, context)));
                });

			//Bug #2284: don't forget to set the IncludeWorkingFacility property
        	worklist.FacilityFilter.IncludeWorkingFacility = data.Filters.Facilities.IncludeWorkingFacility;

            ImportFilter(
                worklist.FacilityFilter,
                data.Filters.Facilities,
                delegate(WorklistData.EnumValueData s)
                {
                    FacilitySearchCriteria criteria = new FacilitySearchCriteria();
                    criteria.Code.EqualTo(s.Code);

                    IFacilityBroker broker = context.GetBroker<IFacilityBroker>();
                    return CollectionUtils.FirstElement(broker.Find(criteria));
                });

            ImportFilter(
                worklist.OrderPriorityFilter,
                data.Filters.OrderPriorities,
                delegate(WorklistData.EnumValueData s)
                {
                    IEnumBroker broker = context.GetBroker<IEnumBroker>();
                    return broker.Find<OrderPriorityEnum>(s.Code);
                });

            ImportFilter(
                worklist.PatientClassFilter,
                data.Filters.PatientClasses,
                delegate(WorklistData.EnumValueData s)
                {
                    IEnumBroker broker = context.GetBroker<IEnumBroker>();
                    return broker.Find<PatientClassEnum>(s.Code);
                });

			ImportFilter(
				worklist.PatientLocationFilter,
				data.Filters.PatientLocations,
				delegate(WorklistData.LocationData s)
				{
					LocationSearchCriteria criteria = new LocationSearchCriteria();
					criteria.Id.EqualTo(s.Id);

					ILocationBroker broker = context.GetBroker<ILocationBroker>();
					return CollectionUtils.FirstElement(broker.Find(criteria));
				});

            worklist.PortableFilter.IsEnabled = data.Filters.Portable.Enabled;
            worklist.PortableFilter.Value = data.Filters.Portable.Value;

			//Bug #2429: don't forget to include the time filter
			worklist.TimeFilter.IsEnabled = data.Filters.TimeWindow.Enabled;
        	worklist.TimeFilter.Value = data.Filters.TimeWindow == null || data.Filters.TimeWindow.Value == null
        	                            	? null
        	                            	: data.Filters.TimeWindow.Value.CreateTimeRange();
        }

        #endregion

        #region Helpers

        private void ExportFilter<TDomain, TData>(WorklistMultiValuedFilter<TDomain> filter, WorklistData.MultiValuedFilterData<TData> data,
            Converter<TDomain, TData> converter)
        {
            data.Enabled = filter.IsEnabled;
            data.Values = CollectionUtils.Map(filter.Values, converter);
        }

        private void ImportFilter<TDomain, TData>(WorklistMultiValuedFilter<TDomain> filter, WorklistData.MultiValuedFilterData<TData> data,
            Converter<TData, TDomain> converter)
        {
            if (data != null)
            {
                filter.IsEnabled = data.Enabled;
                foreach (TData i in data.Values)
                {
                    TDomain value = converter(i);
                    if (value != null)
                        filter.Values.Add(value);
                }
            }
        }

        private Worklist LoadOrCreateWorklist(string name, string worklistClassName, IPersistenceContext context)
        {
            Worklist worklist;

            try
            {
                worklist = context.GetBroker<IWorklistBroker>().FindWorklist(name, worklistClassName);
            }
            catch (EntityNotFoundException)
            {
                worklist = WorklistFactory.Instance.CreateWorklist(worklistClassName);
                worklist.Name = name;

                context.Lock(worklist, DirtyState.New);
            }

            return worklist;
        }

        #endregion

    }
}
