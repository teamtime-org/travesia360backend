namespace TeamTime.Application.Mappings;

public interface IMapper<TSource, TDestination>
{
    TDestination Map(TSource source);
    IEnumerable<TDestination> MapList(IEnumerable<TSource> source);
}